using Core.Constant;
using Core.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Core.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Utils
{
    public static class BaseUtil
    {
        public static Expression<Func<T, bool>> AddSearchCriteria<T>(FilterDto model, Expression<Func<T, bool>> predicate)
        {
            if (string.IsNullOrEmpty(model.SearchBy) || model.SearchBy == "[]")
                return predicate;

            IList<SearchParams> filters = JsonConvert.DeserializeObject<List<SearchParams>>(model.SearchBy);
            if (filters == null || !filters.Any())
                return predicate;

            var parameterExp = Expression.Parameter(typeof(T), "x");
            var propertyCache = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p);

            foreach (var criteria in filters)
            {
                if (!propertyCache.TryGetValue(criteria.Key, out var propertyInfo))
                    continue;

                var propertyExp = Expression.Property(parameterExp, propertyInfo);
                var propertyType = propertyInfo.PropertyType;

                Expression comparisonExp = null;
                var value = criteria.Value;

                //if (propertyType == typeof(string))
                //{
                //    comparisonExp = BuildStringComparison(propertyExp, value);
                //    var lambda = Expression.Lambda<Func<T, bool>>(comparisonExp, parameterExp);
                //    predicate = predicate.And(lambda);
                //}
                //else if (TryBuildComparison(propertyExp, value, propertyType, out comparisonExp))
                //{
                //    var lambda = Expression.Lambda<Func<T, bool>>(comparisonExp, parameterExp);
                //    predicate = predicate.And(lambda);
                //}
            }

            return predicate;
        }

        private static Expression BuildStringComparison(Expression propertyExp, string value)
        {
            var valueExp = Expression.Constant(value.ToLower());
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var propertyToLowerCase = Expression.Call(propertyExp, toLowerMethod);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            return Expression.Call(propertyToLowerCase, containsMethod, valueExp);
        }

        private static bool TryBuildComparison(Expression propertyExp, string value, Type propertyType, out Expression comparisonExp)
        {
            comparisonExp = null;

            if (propertyType == typeof(int) && int.TryParse(value, out int intValue))
            {
                comparisonExp = Expression.Equal(propertyExp, Expression.Constant(intValue));
                return true;
            }
            if (propertyType == typeof(int?) && int.TryParse(value, out intValue))
            {
                comparisonExp = Expression.Equal(propertyExp, Expression.Constant((int?)intValue, typeof(int?)));
                return true;
            }
            if (propertyType == typeof(bool) && bool.TryParse(value, out bool boolValue))
            {
                comparisonExp = Expression.Equal(propertyExp, Expression.Constant(boolValue));
                return true;
            }
            if (propertyType == typeof(DateTime) && DateTime.TryParse(value, out DateTime dateTimeValue))
            {
                comparisonExp = Expression.Equal(propertyExp, Expression.Constant(dateTimeValue.Date));
                return true;
            }
            if (propertyType == typeof(DateTime?) && DateTime.TryParse(value, out dateTimeValue) && propertyExp.ToString().Replace("x.", "").StartsWith("Create"))
            {
                // Create the expression for greater than or equal to the start of the day
                var greaterThanOrEqualExp = Expression.GreaterThanOrEqual(
                    propertyExp,
                    Expression.Constant((DateTime?)dateTimeValue.Date, typeof(DateTime?))
                );

                // Create the expression for less than or equal to the end of the day
                var lessThanOrEqualExp = Expression.LessThanOrEqual(
                    propertyExp,
                    Expression.Constant((DateTime?)dateTimeValue.Date.AddDays(1).AddSeconds(-1), typeof(DateTime?))
                );

                // Combine the two expressions using logical AND (AndAlso)
                comparisonExp = Expression.AndAlso(greaterThanOrEqualExp, lessThanOrEqualExp);

                return true;
            }
            if (propertyType == typeof(DateTime?) && DateTime.TryParse(value, out dateTimeValue))
            {
                var opt = propertyExp.ToString().Replace("x.", "");
                comparisonExp = opt switch
                {
                    "ValidFrom" => Expression.GreaterThanOrEqual(propertyExp, Expression.Constant((DateTime?)dateTimeValue.Date, typeof(DateTime?))),
                    "ValidTo" => Expression.LessThanOrEqual(propertyExp, Expression.Constant((DateTime?)dateTimeValue.Date, typeof(DateTime?))),
                    "startDate" => Expression.GreaterThanOrEqual(propertyExp, Expression.Constant((DateTime?)dateTimeValue.Date, typeof(DateTime?))),
                    "endDate" => Expression.LessThanOrEqual(propertyExp, Expression.Constant((DateTime?)dateTimeValue.Date, typeof(DateTime?))),
                    _ => Expression.Equal(propertyExp, Expression.Constant((DateTime?)dateTimeValue.Date, typeof(DateTime?)))
                };
                return true;
            }

            return false;
        }
        //public static Expression<Func<T, bool>> AddSearchCriteria<T>(FilterVM model)
        //{
        //    Expression<Func<T, bool>> predicate = x=> true;

        //    if (!string.IsNullOrEmpty(model.SearchBy))
        //    {
        //        IList<SearchParams> filters = JsonConvert.DeserializeObject<List<SearchParams>>(model.SearchBy);

        //        // Apply advanced search criteria from SearchBy
        //        if (filters != null && filters.Any())
        //        {
        //            foreach (var criteria in filters)
        //            {
        //                var parameterExp = Expression.Parameter(typeof(T), "x");
        //                var propertyExp = Expression.Property(parameterExp, criteria.Key);
        //                var valueExp = Expression.Constant(criteria.Value.ToLower());
        //                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        //                var propertyToLowerCase = Expression.Call(propertyExp, toLowerMethod);
        //                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        //                var containsExpression = Expression.Call(propertyToLowerCase, containsMethod, valueExp);

        //                var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameterExp);

        //                predicate = predicate.And(lambda);
        //            }

        //        }
        //    }
        //    return predicate;

        //}
        //public static Expression<Func<T, bool>> AddSearchCriteria<T>(FilterVM model, Expression<Func<T, bool>> predicate)
        //{
        //    //Expression<Func<T, bool>> predicate = x => true;

        //    if (!string.IsNullOrEmpty(model.SearchBy) && model.SearchBy != "[]")
        //    {
        //        IList<SearchParams> filters = JsonConvert.DeserializeObject<List<SearchParams>>(model.SearchBy);
        //        if (filters != null && filters.Any())
        //        {
        //            foreach (var criteria in filters)
        //            {
        //                var parameterExp = Expression.Parameter(typeof(T), "x");
        //                var propertyExp = Expression.Property(parameterExp, criteria.Key);
        //                var propertyType = typeof(T).GetProperty(criteria.Key).PropertyType;
        //                Expression comparisonExp = null;

        //                if (propertyType == typeof(string))
        //                {
        //                    var valueExp = Expression.Constant(criteria.Value.ToLower());
        //                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        //                    var propertyToLowerCase = Expression.Call(propertyExp, toLowerMethod);
        //                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        //                    comparisonExp = Expression.Call(propertyToLowerCase, containsMethod, valueExp);
        //                }
        //                else if (propertyType == typeof(int))
        //                {
        //                    var valueExp = Expression.Constant(int.Parse(criteria.Value));
        //                    comparisonExp = Expression.Equal(propertyExp, valueExp);
        //                }
        //                else if (propertyType == typeof(int?))
        //                {
        //                    var valueExp = Expression.Constant((int?)int.Parse(criteria.Value), typeof(int?));
        //                    comparisonExp = Expression.Equal(propertyExp, valueExp);
        //                }
        //                else if (propertyType == typeof(bool))
        //                {
        //                    var valueExp = Expression.Constant(bool.Parse(criteria.Value));
        //                    comparisonExp = Expression.Equal(propertyExp, valueExp);
        //                }
        //                else if (propertyType == typeof(DateTime))
        //                {
        //                    var valueExp = Expression.Constant(DateTime.Parse(criteria.Value));
        //                    comparisonExp = Expression.Equal(propertyExp, valueExp);
        //                }
        //                else if (propertyType == typeof(DateTime?))
        //                {
        //                    var valueExp = Expression.Constant(DateTime.Parse(criteria.Value).Date);
        //                    if (valueExp!=null&&valueExp.Value!=null)
        //                        comparisonExp = Expression.Equal(propertyExp, valueExp);
        //                }
        //                if (comparisonExp != null)
        //                {
        //                    var lambda = Expression.Lambda<Func<T, bool>>(comparisonExp, parameterExp);
        //                    predicate = predicate.And(lambda);
        //                }
        //            }
        //        }
        //    }
        //    return predicate;
        //}

        private static bool IsNotDateNumberOrBool(string value)
        {
            if (DateTime.TryParse(value, out _))
                return false;

            if (bool.TryParse(value, out _))
                return false;

            if (int.TryParse(value, out _))
                return false;

            if (double.TryParse(value, out _))
                return false;

            return true;
        }
        public static string GenerateSearchQuery<T>(string searchValue)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            string query = "";

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(string) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int))
                {
                    query += $"{property.Name} like '%{searchValue}%' OR ";
                }
            }

            query = query.TrimEnd(" OR ".ToCharArray()); // Remove trailing " OR "

            return query;
        }
        // Generate random digit code
        public static string GenerateRandomDigitCode(int length)
        {
            using var random = new SecureRandomNumberGenerator();
            var str = string.Empty;
            for (var i = 0; i < length; i++)
                str = string.Concat(str, random.Next(10).ToString());
            return str;
        }

        // Returns an random integer number within a specified rage
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            using var random = new SecureRandomNumberGenerator();
            return random.Next(min, max);
        }
        #region SendMessage
        public static bool SendMessage(string message, string number)
        {
            SqlDataReader cmdReader;
            var con = new SqlConnection(Config.config.GetSection("ConnectionStringsForMessage").GetSection("DawlanceContext").Value);
            try
            {
                con.Open();
                SqlCommand cmdProc = new(@$"EXEC [SP_SMS_Svc_SalesmanApp_Common] '" + number + "' , '" + message + "'", con);
                cmdProc.CommandTimeout = 100;
                cmdReader = cmdProc.ExecuteReader();
                if (cmdReader.HasRows)
                {
                    cmdReader.Close();
                    return true;
                }
                cmdReader.Close();
            }
            catch
            {
                return false;
            }
            finally
            { con.Close(); }
            return false;
        }
        #endregion

        public static string GetRawUrlForScan(HttpRequest request, string code)
        {
            var httpContext = request.HttpContext;
            return $"{request.Headers.FirstOrDefault(x => x.Key == SiteUrl.Referer).Value}{Config.config.GetSection("RedirectLink").Value}/{code}";
        }
        public static string GetRawUrl(HttpRequest request)
        {
            var httpContext = request.HttpContext;
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        }
        public static string GetRawUrlComplete(HttpRequest request)
        {
            var httpContext = request.HttpContext;
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
        }
      
    }
}
