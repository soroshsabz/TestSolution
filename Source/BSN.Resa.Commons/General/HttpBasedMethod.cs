using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BSN.Resa.Commons
{
    using Parameter = HttpBasedMethodParameter;
    using ParameterSite = HttpBasedMethodParameterSite;

    public struct HttpBasedMethodParameter
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public ParameterSite Site { get; private set; }

        public bool IsOptional { get; set; }

        public HttpBasedMethodParameter(string name, Type type, ParameterSite site, bool isOptional)
        {
            Site = site;
            Name = name;
            Type = type;
            IsOptional = isOptional;

            if (site == ParameterSite.PathParameter && isOptional)
                throw new InvalidOperationException("Path parameter can't be optional.");
        }
    }

    public enum HttpBasedMethodParameterSite
    {
        QueryString,
        PathParameter,
        Content
    }

    public class HttpBasedMethod<TResult>
    {
        public readonly static List<HttpMethod> AllowedMethods = new List<HttpMethod>() { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, new HttpMethod("PATCH"), HttpMethod.Delete };

        public static implicit operator Delegate(HttpBasedMethod<TResult> method) => method.Invoke;

        #region Properties

        protected string Address { get; set; }

        protected HttpMethod Method { get; set; }
        
        public IReadOnlyCollection<Parameter> Parameters => _parameters.ToList().AsReadOnly();

        #endregion

        public HttpBasedMethod(string address, HttpMethod method)
        {
            if (!AllowedMethods.Contains(method))
                throw new NotSupportedException("Http method not supported.");

            Address = address;
            Method = method;

            _parameters = new List<Parameter>();
        }

        public HttpBasedMethod<TResult> AddParameter(string name, Type type, ParameterSite site, bool isOptional = false)
        {
            return AddParameter(new Parameter(name, type, site, isOptional));
        }

        public HttpBasedMethod<TResult> AddParameter(Parameter parameter)
        {
            if (parameter.Site == ParameterSite.Content && Method != HttpMethod.Post && Method != HttpMethod.Put && Method != new HttpMethod("PATCH"))
                throw new NotSupportedException($"Parameter site 'Content' not supported for http method '{Method.Method}'.");

            if (parameter.Site == ParameterSite.Content && _parameters.Any(x => x.Site == ParameterSite.Content))
                throw new ArgumentException($"A parameter with parameter site 'Content' already exists.");

            if (_parameters.Any(x => x.Name == parameter.Name))
                throw new ArgumentException("A parameter with this name already exists.");

            _parameters.Add(parameter);

            return this;
        }

        public delegate Task<TResult> Delegate(params (string name, object value)[] arguments);

        public async Task<TResult> Invoke(params (string name, object value)[] arguments)
        {
            IDictionary<string, object> args = arguments.ToDictionary(x => x.name, x => x.value);

            string query = string.Empty;
            string path = Address;
            object content = null;

            foreach (Parameter param in _parameters)
            {
                if (!args.ContainsKey(param.Name))
                {
                    if (param.IsOptional)
                        continue;
                    throw new ArgumentNullException($"Parameter '{param.Name}' is not set.");
                }
                        
                object value = args[param.Name];
                if (value != null && !param.Type.IsAssignableFrom(value.GetType()))
                    throw new ArgumentException();

                switch (param.Site)
                {
                    case ParameterSite.PathParameter:
                        path = path.Replace("{" + param.Name + "}", value?.ToString());
                        break;
                    case ParameterSite.QueryString:
                        query = (value == null ? query : (query + $"{(string.IsNullOrEmpty(query) ? string.Empty : "&")}{param.Name}={value.ToString()}"));
                        break;
                    case ParameterSite.Content:
                        content = value;
                        break;
                }
            }

            var request = ResaHttpRequest.To(path);
            HttpResponseMessage response = null;

            if (Method == HttpMethod.Get)
            {
                response = await request.GetAsync("", query).ConfigureAwait(false);
            }
            else if (Method == HttpMethod.Post)
            {
                response = await request.PostAsJsonAsync("", content, query).ConfigureAwait(false);
            }
            else if (Method == HttpMethod.Put)
            {
                response = await request.PutAsJsonAsync("", content, query).ConfigureAwait(false);
            }
            else if (Method == new HttpMethod("PATCH"))
            {
                response = await request.PatchAsJsonAsync("", content, query).ConfigureAwait(false);
            }
            else if (Method == HttpMethod.Delete)
            {
                response = await request.DeleteAsync("", query).ConfigureAwait(false);
            }

            if (typeof(Void) == typeof(TResult))
                return default(TResult);

            return response.Content.ReadAsAsync<TResult>().Result;
        }

        private ICollection<Parameter> _parameters;
    }

    public sealed class Void
    {
        private Void() { }
    }
}