namespace Monkey.Core
{
    public class Lexer
    {
        readonly string _source;
        private int _position; // Position in source where last character was read.
        private int _readPosition; // Position in source where next character is read.
        private char _ch;

        TokenType _Keyword(string ident)
        {
            switch (ident)
            {
                case "true":
                    return TokenType.True;
                case "false":
                    return TokenType.False;
                default:
                    return TokenType.Ident;
            }
        }

        public Lexer(string source)
        {
            this._source = source;
            this._position = 0;
            this._readPosition = 0;
        }

        public Token NextToken()
        {
            Token tok;
            SkipWhiteSpace();

            switch (_ch)
            {
                case '=':
                    tok = new Token(TokenType.Assign, "+");
                    break;
                case '%':
                    tok = new Token(TokenType.MOD, "+");
                    break;
                case '+':
                    tok = new Token(TokenType.PLUS, "+");
                    break;
                case '-':
                    tok = new Token(TokenType.MINUS, "-");
                    break;
                case '*':
                    tok = new Token(TokenType.MULTIPLY, "+");
                    break;
                case '/':
                    tok = new Token(TokenType.DIVIDE, "-");
                    break;
                case '"':
                    string _string = ReadString();
                    if (_ch == '\0')
                        tok = new Token(TokenType.Illegal, _string);
                    else
                        tok = new Token(TokenType.String, _string);
                    break;
                default:
                    if (IsDigital(_ch))
                        return new Token(TokenType.Number, ReadNumber());
                    else if (IsLetter(_ch))
                    {
                        string literal = ReadIdentifier();
                        return new Token(_Keyword(literal), literal);
                    }
                    else
                        tok = new Token(TokenType.Illegal, $"{_ch}");
                    break;
            }
            ReadChar();
            return tok;
        }

        private char peekChar => _readPosition >= _source.Length ? '\0' : _source[_readPosition];

        private void ReadChar()
        {
            _ch = _readPosition >= _source.Length ? '\0' : _source[_readPosition];
            _position = _readPosition;
            _readPosition++;
        }

        private void SkipWhiteSpace()
        {
            while (_ch == ' ' || _ch == '\n' || _ch == '\t' || _ch == '\r')
                ReadChar();
        }

        private string ReadString()
        {
            // "Hellow"
            // p = H
            // _position = last "
            int p = _position + 1;
            do
            {
                ReadChar();
            } while (_ch != '"' && _ch != '\0');

            if (p == _position) return "";

            return _source.Substring(p, _position - p);
        }

        private string ReadNumber()
        {
            // 12345
            // p = 1
            int p = _position;
            while (IsDigital(_ch) || _ch == '.' && IsDigital(peekChar))
                ReadChar();

            return _source.Substring(p, _position - p + 1);
        }

        private string ReadIdentifier()
        {
            int p = _position;
            while (IsLetter(_ch))
                ReadChar();

            return _source.Substring(p, _position - p + 1);
        }

        private bool IsDigital(char ch) => ch >= '0' && ch <= '9';

        private bool IsLetter(char ch) => ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '_';
    }
}