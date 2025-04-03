// Result is a rough implementation of an Optional/Result/Error Union type in C# that you find other modern 
// langauges (Go, Rust, Zig). It allows for better and more explicit error handling and less opaque flow 
// control, while avoiding the level of abstraction you encounter when dealing with monads commonly found 
// in functional langauges.  

namespace Lotto;

public abstract record Result<T>
{
    public static Result<T> Ok(T value) => new OkResult<T>(value);
    
    public static implicit operator Result<T>(T value) => Ok(value);
}

// Delcared here ar the Result type for example if you wanted an Error that indicates a timeout you can
// add: public record TimeOutResult<T> : ErrorResult<T>;

public record OkResult<T>(T Value) : Result<T>;

public abstract record ErrorResult<T> : Result<T>;