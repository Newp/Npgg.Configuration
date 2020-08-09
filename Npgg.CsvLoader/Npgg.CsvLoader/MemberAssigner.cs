//source : https://www.codeproject.com/Articles/993798/FieldInfo-PropertyInfo-GetValue-SetValue-Alternati
//lisence : The Code Project Open License(CPOL)

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Npgg.CsvLoader
{
    public class MemberAssigner : IGetSet
    {
        private readonly static MethodInfo sm_valueAssignerMethod
            = typeof(MemberAssigner).GetMethod("ValueAssigner", BindingFlags.Static | BindingFlags.NonPublic);

        private static void ValueAssigner<T>(out T dest, T src) => dest = src;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;


        public object GetValue(object targetObject) => getter(targetObject);

        public void SetValue(object targetObject, object memberValue) => setter(targetObject, memberValue);


        public MemberAssigner(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new ArgumentNullException("Must initialize with a non-null Field or Property");

            MemberExpression exMember = null;

            if (memberInfo is FieldInfo fi)
            {
                var assignmentMethod = sm_valueAssignerMethod.MakeGenericMethod(fi.FieldType);

                Init(fi.DeclaringType
                    ,fi.FieldType
                    ,_ex => exMember = Expression.Field(_ex, fi) 
                    ,(_, _val) => Expression.Call(assignmentMethod, exMember, _val) 
                    ,out this.getter
                    ,out this.setter
                );
            }
            else if (memberInfo is PropertyInfo pi)
            {
                var assignmentMethod = pi.GetSetMethod(true);

                Init(pi.DeclaringType
                    , pi.PropertyType
                    ,_ex => exMember = Expression.Property(_ex, pi)
                    ,(_obj, _val) => Expression.Call(_obj, assignmentMethod, _val) 
                    ,out this.getter
                    ,out this.setter
                );
            }
            else
            {
                throw new ArgumentException
                ("The member must be either a Field or a Property, not " + memberInfo.MemberType);
            }
        }

        private void Init(
            Type objectType,
            Type valueType,
            Func<Expression, MemberExpression> getMember,
            Func<Expression, Expression, MethodCallExpression> makeCallExpression,
            out Func<object, object> getter,
            out Action<object, object> setter)
        {
            var exObjParam = Expression.Parameter(typeof(object), "theObject");
            var exValParam = Expression.Parameter(typeof(object), "theProperty");

            var exObjConverted = Expression.Convert(exObjParam, objectType);
            var exValConverted = Expression.Convert(exValParam, valueType);

            Expression exMember = getMember(exObjConverted);

            Expression getterMember = valueType.IsValueType ? Expression.Convert(exMember, typeof(object)) : exMember;
            getter = Expression.Lambda<Func<object, object>>(getterMember, exObjParam).Compile();

            Expression exAssignment = makeCallExpression(exObjConverted, exValConverted);
            setter = Expression.Lambda<Action<object, object>>(exAssignment, exObjParam, exValParam).Compile();
        }

    }

}
