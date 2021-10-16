using System;
using System.Reflection;
using M5x.Couch.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Couch
{
    public class CouchDocumentWrapper<T> : ICouchDocument, ISelfContained
    {
        private readonly MemberWrapper _id;
        private readonly MemberWrapper _rev;
        private readonly JsonSerializer _serializer = new();

        protected CouchDocumentWrapper()
        {
            _rev = GetFunc("_rev") ?? GetFunc("rev");
            _id = GetFunc("_id") ?? GetFunc("id");
        }

        public CouchDocumentWrapper(Func<T> ctor) : this()
        {
            Instance = ctor();
        }

        public CouchDocumentWrapper(T instance) : this()
        {
            Instance = instance;
        }

        /// <summary>
        ///     Gets a function that accesses the value of a property or field
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static MemberWrapper GetFunc(string name)
        {
            var members = typeof(T).GetMember(name,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (members == null || members.Length == 0)
                return null;

            var prop = members[0] as PropertyInfo;
            if (prop == null)
            {
                var field = members[0] as FieldInfo;
                if (field == null)
                    return null;

                return new MemberWrapper { Field = field };
            }

            return new MemberWrapper { Property = prop };
        }

        private class MemberWrapper
        {
            public FieldInfo Field;
            public PropertyInfo Property;

            public void SetValue(object instance, object value)
            {
                if (Field == null)
                    Property.SetValue(instance, value, null);
                else
                    Field.SetValue(instance, value);
            }

            public object GetValue(object instance)
            {
                if (Field == null) return Property.GetValue(instance, null);

                return Field.GetValue(instance);
            }
        }

        #region ICouchDocument Members

        public T Instance { get; private set; }

        public string Rev
        {
            get
            {
                if (_rev == null)
                    return null;

                return (string)_rev.GetValue(Instance);
            }
            set
            {
                if (_rev == null)
                    return;

                _rev.SetValue(Instance, value);
            }
        }

        public string Id
        {
            get
            {
                if (_id == null)
                    return null;

                return (string)_id.GetValue(Instance);
            }
            set
            {
                if (_id == null)
                    return;

                _id.SetValue(Instance, value);
            }
        }

        #endregion

        #region ICanJson Members

        public void WriteJson(JsonWriter writer)
        {
            if (Id == null)
            {
                var tokenWriter = new JTokenWriter();
                _serializer.Serialize(tokenWriter, Instance);
                var obj = (JObject)tokenWriter.Token;
                obj.Remove("_rev");
                obj.Remove("_id");
                obj.WriteTo(writer);
            }
            else
            {
                _serializer.Serialize(writer, Instance);
            }
        }

        public void ReadJson(JObject obj)
        {
            Instance = (T)_serializer.Deserialize(new JTokenReader(obj), typeof(T));
            _id.SetValue(Instance, obj["_id"].Value<string>());
            _rev.SetValue(Instance, obj["_rev"].Value<string>());
        }

        #endregion
    }
}