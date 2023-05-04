using Osmosis.Models;
using System.Reflection;

namespace Osmosis.Services
{
    public class GenericServices
    {
        public static void CopyProperties<T, U>(T oldObject, U newObject)
        {
            PropertyInfo[] newProperties = newObject.GetType().GetProperties();
            PropertyInfo[] oldProperties = oldObject.GetType().GetProperties();
            var emptyObject = (U)Activator.CreateInstance(typeof(U));

            foreach (PropertyInfo newProp in newProperties)
            {
                if (newProp.Name.Equals("id"))
                    continue;

                var newValue = newProp.GetValue(newObject);
                var emptyValue = newProp.GetValue(emptyObject);
                if (newValue == null || newValue.Equals(emptyValue))
                    continue;

                foreach (PropertyInfo oldProp in oldProperties)
                {
                    if (oldProp.Name.Equals("id"))
                        continue;

                    if (newProp.Name == oldProp.Name)
                    {
                        oldProp.SetValue(oldObject, newProp.GetValue(newObject));
                        break;
                    }
                }
            }
        }
    }
}
