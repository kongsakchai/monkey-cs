namespace Monkey.Core;

public class Lexer
{
    private string _source;
    private int _position; // Position in source where last character was read.
    private int _readPosition; // Position in source where next character is read.
    private char _ch;

    TokenType _keyword(string ident)
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
        ReadChar();
    }

    public Token NextToken()
    {
        Token tok;
        SkipWhiteSpace();

        switch (_ch)
        {
            case '\0':
                tok = new Token(TokenType.Eof, "\0");
                break;
            case '\n':
                tok = new Token(TokenType.Eol, "\n");
                break;
            case '!':
                if (peekChar == '=')
                {
                    tok = new Token(TokenType.NotEq, "!=");
                    ReadChar();
                }
                else
                    tok = new Token(TokenType.Not, "!");
                break;
            case '=':
                if (peekChar == '=')
                {
                    tok = new Token(TokenType.Equal, "==");
                    ReadChar();
                }
                else
                    tok = new Token(TokenType.Assign, "=");
                break;
            case '>':
                if (peekChar == '=')
                {
                    tok = new Token(TokenType.GreaterEq, ">=");
                    ReadChar();
                }
                else
                    tok = new Token(TokenType.Greater, ">");
                break;
            case '<':
                if (peekChar == '=')
                {
                    tok = new Token(TokenType.LessEq, "<=");
                    ReadChar();
                }
                else
                    tok = new Token(TokenType.Less, "<");
                break;
            case '&':
                if (peekChar == '&')
                {
                    tok = new Token(TokenType.AND, "&&");
                    ReadChar();
                }
                else
                    tok = new Token(TokenType.Illegal, $"{_ch}");
                break;
            case '|':
                if (peekChar == '|')
                {
                    tok = new Token(TokenType.OR, "||");
                    ReadChar();
                }
                else
                    tok = new Token(TokenType.Illegal, $"{_ch}");
                break;
            case '%':
                tok = new Token(TokenType.Mod, "%");
                break;
            case '+':
                tok = new Token(TokenType.Add, "+");
                break;
            case '-':
                tok = new Token(TokenType.Sub, "-");
                break;
            case '*':
                tok = new Token(TokenType.Multiply, "*");
                break;
            case '/':
                tok = new Token(TokenType.Divide, "/");
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
                    return new Token(_keyword(literal), literal);
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
        while (_ch == ' ' || _ch == '\t' || _ch == '\r')
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

        return _source.Substring(p, _position - p);
    }

    private string ReadIdentifier()
    {
        // ab=1
        // 0123
        // p=a
        int p = _position;
        while (IsLetter(_ch))
            ReadChar();

        return _source.Substring(p, _position - p);
    }

    private bool IsDigital(char ch) => ch >= '0' && ch <= '9';

    private bool IsLetter(char ch) => ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z';
}