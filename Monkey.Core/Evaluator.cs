namespace Monkey.Core;

public static class Evaluator
{
    static readonly NullObject Null = new();
    static readonly BooleanObject True = new(true);
    static readonly BooleanObject False = new(false);

    public static IObject Eval(Node node, Environment env)
    {
        switch (node)
        {
            case Program p:
                return EvalProgram(p.Statements, env);
            case ExpressionStatement es:
                return Eval(es.Expression, env);
            case LetStatement ls:
                {
                    var name = ls.Name.Value;
                    var value = Eval(ls.Value, env);
                    return IsError(value) ? value : env.Set(name, value);
                }

            // === Expression ===

            case PrefixExpression pe:
                {
                    var right = Eval(pe.Right, env);
                    return IsError(right) ? right : EvalPrefixExpression(pe.Operator, right);
                }
            case InfixExpression ie:
                {
                    var left = Eval(ie.Left, env);
                    if (IsError(left)) return left;
                    var right = Eval(ie.Right, env);
                    return IsError(right) ? right : EvalInfixExpression(ie.Operator, left, right);
                }
            case AssignExpression ae:
                {
                    var name = ae.Name.Value;
                    var value = Eval(ae.Value, env);
                    return IsError(value) ? value : env.Set(name, value);
                }
            case Identifier _ident:
                return EvalIdentifier(_ident, env);
            case NumberLiteral _number:
                return new NumberObject(_number.Value);
            case StringLiteral _string:
                return new StringObject(_string.Value);
            case BooleanLiteral _boolean:
                return (_boolean.Value) ? True : False;
            default:
                return new ErrorObject($"Unkown statement : {node.String}");
        }
    }

    private static IObject EvalProgram(List<Statement> statements, Environment env)
    {
        IObject result = Null;
        for (int i = 0; i < statements.Count; i++)
        {
            result = Eval(statements[i], env);

            if (result is ErrorObject e)
                return e;
        }
        return result;
    }

    private static IObject EvalIdentifier(Identifier ident, Environment env)
    {
        var (val, ok) = env.Get(ident.Value);
        if (ok)
            return val!;
        return new ErrorObject($"Identifier not found: {ident.Value}");
    }

    private static IObject EvalPrefixExpression(string op, IObject right)

    {
        if (right.Type == ObjectType.Number)
            return EvalNumberPrefixExpression(op, right);

        if (op == "!")
            return EvalNotPrefixExpression(right);

        return new ErrorObject($"Unknown operator: {op} {right.Type}");
    }

    private static IObject EvalNumberPrefixExpression(string op, IObject right)
    {
        if (op == "-")
        {
            var _right = ((NumberObject)right).Value;
            return new NumberObject(-_right);
        }

        if (op == "+")
            return right;

        return new ErrorObject($"Unknown operator: {op} {right.Type}");
    }

    private static IObject EvalNotPrefixExpression(IObject right)
    {
        if (ReferenceEquals(right, True))
            return False;
        if (ReferenceEquals(right, False))
            return True;
        if (ReferenceEquals(right, Null))
            return True;

        return new ErrorObject($"Unknown operator: ! {right.Type}");
    }

    private static IObject EvalInfixExpression(string op, IObject left, IObject right)
    {
        // === Number ===
        if (left.Type == ObjectType.Number && right.Type == ObjectType.Number)
            return EvalNumberInfixExpression(op, left, right);

        // === String ===
        if (left.Type == ObjectType.String || right.Type == ObjectType.String)
            return EvalStringInfixExpression(op, left, right);

        // === Boolean ===
        if (left.Type == ObjectType.Boolean && right.Type == ObjectType.Boolean)
            return EvalBooleanInfixExpression(op, left, right);

        // === Error ===
        return new ErrorObject($"Unknown operator: {left.Type} {op} {right.Type}");
    }

    private static IObject EvalNumberInfixExpression(string op, IObject left, IObject right)
    {
        var _left = ((NumberObject)left).Value;
        var _right = ((NumberObject)right).Value;

        return op switch
        {
            "+" => new NumberObject(_left + _right),
            "-" => new NumberObject(_left - _right),
            "*" => new NumberObject(_left * _right),
            "/" => new NumberObject(_left / _right),
            "%" => new NumberObject(_left % _right),
            "==" => (_left == _right) ? True : False,
            "!=" => (_left != _right) ? True : False,
            ">" => (_left > _right) ? True : False,
            ">=" => (_left >= _right) ? True : False,
            "<" => (_left < _right) ? True : False,
            "<=" => (_left <= _right) ? True : False,
            _ => new ErrorObject($"Unknown operator: {left.Type} {op} {right.Type}")
        };
    }

    private static IObject EvalBooleanInfixExpression(string op, IObject left, IObject right)
    {

        if (op == "==")
            return (left == right) ? True : False;
        if (op == "!=")
            return (left != right) ? True : False;

        var _left = ((BooleanObject)left).Value;
        var _right = ((BooleanObject)right).Value;

        return op switch
        {
            "&&" => (_left && _right) ? True : False,
            "||" => (_left || _right) ? True : False,
            _ => new ErrorObject($"Unknown operator: {left.Type} {op} {right.Type}")
        };
    }

    private static IObject EvalStringInfixExpression(string op, IObject left, IObject right)
    {
        if (op != "+")
            return new ErrorObject($"Unknown operator: {left.Type} {op} {right.Type}");

        var _left = left.String;
        var _right = right.String;

        return new StringObject($"{_left}{_right}");
    }

    private static bool IsError(IObject obj) => obj.Type == ObjectType.Error;
}
