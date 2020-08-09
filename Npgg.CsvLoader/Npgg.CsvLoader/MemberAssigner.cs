//source : https://www.codeproject.com/Articles/993798/FieldInfo-PropertyInfo-GetValue-SetValue-Alternati
//lisence : The Code Project Open License(CPOL)

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Npgg.CsvLoader
{
    public class MemberAssigner
    {
        private readonly static MethodInfo sm_valueAssignerMethod
            = typeof(MemberAssigner).GetMethod("ValueAssigner", BindingFlags.Static | BindingFlags.NonPublic);

        private static void ValueAssigner<T>(out T dest, T src) => dest = src;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;


        public object GetValue(object targetObject) => getter(targetObject);

        public void SetValue(object targetObject, object memberValue) => setter(targetObject, memberValue);

        public readonly Type DeclaringType;
        public readonly Type ValueType;

        public MemberAssigner(MemberInfo memberInfo)
        {
            this.DeclaringType = memberInfo.DeclaringType;
            MemberExpression exMember = null;
            Func<Expression, MemberExpression> getMemberExpression;
            Func<Expression, Expression, MethodCallExpression> makeCallExpression;

            if (memberInfo is FieldInfo fi)
            {
                this.ValueType = fi.FieldType;
                var assignmentMethod = sm_valueAssignerMethod.MakeGenericMethod(fi.FieldType);

                getMemberExpression = _ex => exMember = Expression.Field(_ex, fi);
                makeCallExpression = (_, _val) => Expression.Call(assignmentMethod, exMember, _val);
            }
            else if (memberInfo is PropertyInfo pi)
            {
                this.ValueType = pi.PropertyType;
                var assignmentMethod = pi.GetSetMethod(true);

                getMemberExpression = _ex => exMember = Expression.Property(_ex, pi);
                makeCallExpression = (_obj, _val) => Expression.Call(_obj, assignmentMethod, _val);
            }
            else
            {
                throw new ArgumentException
                ("The member must be either a Field or a Property, not " + memberInfo.MemberType);
            }

            Init( getMemberExpression
                    , makeCallExpression
                    , out this.getter
                    , out this.setter
                );

        }

        private void Init(
            Func<Expression, MemberExpression> getMember,
            Func<Expression, Expression, MethodCallExpression> makeCallExpression,
            out Func<object, object> getter,
            out Action<object, object> setter)
        {
            var exObjParam = Expression.Parameter(typeof(object), "theObject");
            var exValParam = Expression.Parameter(typeof(object), "theProperty");

            var exObjConverted = Expression.Convert(exObjParam, this.DeclaringType);
            var exValConverted = Expression.Convert(exValParam, this.ValueType);

            Expression exMember = getMember(exObjConverted);

            Expression getterMember = ValueType.IsValueType ? Expression.Convert(exMember, typeof(object)) : exMember;
            getter = Expression.Lambda<Func<object, object>>(getterMember, exObjParam).Compile();

            Expression exAssignment = makeCallExpression(exObjConverted, exValConverted);
            setter = Expression.Lambda<Action<object, object>>(exAssignment, exObjParam, exValParam).Compile();
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
            var exValConverted = Expression.Convert(exValParam, ValueType);

            Expression exMember = getMember(exObjConverted);

            Expression getterMember = ValueType.IsValueType ? Expression.Convert(exMember, typeof(object)) : exMember;
            getter = Expression.Lambda<Func<object, object>>(getterMember, exObjParam).Compile();

            Expression exAssignment = makeCallExpression(exObjConverted, exValConverted);
            setter = Expression.Lambda<Action<object, object>>(exAssignment, exObjParam, exValParam).Compile();
        }

    }

}
