namespace Monkey.Core
{
    public class Lexer
    {
        readonly string _source;

        int _posirion;
        int _readPosition;
        char _ch;

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
        }

        public Token NextToken()
        {
            Token tok = new Token(TokenType.Illegal,"");
            return tok;
        }
    }
}