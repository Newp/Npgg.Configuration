
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Npgg.CsvLoader
{

    public interface IGetSet
    {
        object GetValue(object targetObject);
        void SetValue(object targetObject, object memberValue);
    }

    public class GetSet2 : IGetSet
    {
        private Func<object, object> getter;

        private Action<object, object> setter;

        public object GetValue(object targetObject) => getter(targetObject);
        public void SetValue(object targetObject, object memberValue) => setter(targetObject, memberValue);

        public GetSet2(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fi)
            {
                this.getter = fi.GetValue;
                this.setter = fi.SetValue;
            }
            else if (memberInfo is PropertyInfo pi)
            {
                this.getter = pi.GetValue;
                this.setter = pi.SetValue;
            }
            else
            {
                throw new ArgumentException("The member must be either a Field or a Property, not " + memberInfo.MemberType);
            }
        }
    }   

}
