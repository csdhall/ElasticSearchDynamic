using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using ElasticSearch.Net.Utils;
using Microsoft.VisualBasic;

namespace ElasticSearch.Net
{

    public class Hosts
    {
        private readonly dynamic _config;
        string startsWithProtocolRE = @"/^([a-z]+:)?\/\//";

        string[] _urlParseFields = { "protocol", "hostname", "pathname", "port", "auth", "query" };

        string[] _simplify = { "host", "path" };

        private dynamic _defaultPorts = new ExpandoObject();
        private string _protocol;
        private string _host;
        private string _path;
        private int _port;
        private dynamic _auth;
        private dynamic _query;

        public dynamic DefaultPorts
        {
            get { return _defaultPorts; }
            set { _defaultPorts = value; }
        }

        public Hosts(dynamic config)
        {
            _config = config;

            DefaultPorts.http = 80;
            DefaultPorts.https = 443;

            this._protocol = "http";
            this._host = "localhost";
            this._path = "";
            this._port = 9200;
            this._auth = null;
            this._query = null;

            if (config is string)
            {
                if (!Utility.StartsWithProtocol(config as string))
                {
                    config = "http://" + config;
                }
                config = Utility.UrlParse(config as string, false, true, _urlParseFields);
                if (!config.port)
                {
                    var proto = config.protocol || "http";
                    if (proto.charAt(proto.length - 1) == ':')
                    {
                        proto = proto.substring(0, proto.length - 1);
                    }
                    if (DefaultPorts[proto])
                    {
                        config.port = DefaultPorts.defaultPorts[proto];
                    }
                }
            }
            else if (config is Dictionary<string, object>)
            {
                foreach (var value in _simplify)
                {
                    var to = value;
                    var from = to + "name";
                    if (config[from] && config[to])
                    {
                        if (config[to].indexOf(config[from]) == 0)
                        {
                            config[to] = config[from];
                        }
                    }
                    else if (config[from])
                    {
                        config[to] = config[from];
                    }
                    var dictionary = config as Dictionary<string, object>;
                    dictionary.Remove(from);
                    //delete config[from];
                }
            }

            // make sure the query string is parsed
            if (config.query == null)
            {
                // majority case
                config.query = "";
            }

            // make sure that the port is a number
            int parsedPort = 9200;
            int.TryParse(config.port, out parsedPort);
            config.port = parsedPort;

            // make sure the path starts with a leading slash
            if (config.path == "/")
            {
                config.path = "";
            }
            else if (config.path is string && !string.IsNullOrEmpty(config.path) && (config.path as string)[0] != '/')
            {
                config.path = "/" + (config.path || "");
            }

            // strip trailing ":" on the protocol (when config comes from url.parse)
            if (config.protocol.substr(-1) == ":")
            {
                config.protocol = config.protocol.substring(0, config.protocol.length - 1);
            }
        }

        private string MakeUrl(dynamic parameters)
        {
            string port = "";
            if (port != DefaultPorts[_protocol])
            {
                // add an actual port
                port = ":" + _port;
            }

            // build the path
            string path = "" + (_path ?? "") + (parameters.path ?? "");

            // if path doesn"t start with "/" add it.
            if (path[0] != '/')
            {
                path = "/" + path;
            }

            // build the query string
            var query = "";
            if (parameters.query != null)
            {
                // if the user passed in a query, merge it with the defaults from the host
                query = parameters.query is string ? Utility.ParseQueryString(parameters.query) : parameters.query;

            }
            else if (_query != null)
            {
                // just stringify the hosts query
                //TODO
                query = Utility.ParseQueryString(query);
            }

            return _protocol + "://" + _host + port + path + ("?" + query);

        }

    }
}
