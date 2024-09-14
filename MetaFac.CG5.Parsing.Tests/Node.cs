using System;
using System.Linq;
using System.Text;

namespace MetaFac.CG5.Parsing.Tests
{
    public sealed class Node
    {
        private readonly string[]? _errors;
        private readonly StringBuilder? _result;

        public bool HasErrors { get { return _errors is not null; } }
        public string[] Errors => _errors ?? throw new InvalidOperationException();
        public string Result => _result is not null ? _result.ToString() : throw new InvalidOperationException();

        private Node(string[]? errors, StringBuilder? result)
        {
            _errors = errors;
            _result = result;
        }

        public Node(string message)
        {
            _errors = [message];
        }
        public Node(char value)
        {
            _result = new StringBuilder();
            _result.Append(value);
        }

        public static Node Join(Node node1, Node node2, string? separator = null)
        {
            if (node1._errors is not null)
            {
                if (node2._errors is not null)
                {
                    return new Node(node1._errors.Concat(node2._errors).ToArray(), null);
                }
                else
                {
                    return new Node(node1._errors, null);
                }
            }
            else
            {
                if (node2._errors is not null)
                {
                    return new Node(node2._errors, null);
                }
                else
                {
                    // concat chars
                    StringBuilder sb = new StringBuilder();
                    sb.Append(node1._result);
                    if (separator is not null)
                    {
                        sb.Append(separator);
                    }
                    sb.Append(node2._result);
                    return new Node(null, sb);
                }
            }
        }
    }
}
