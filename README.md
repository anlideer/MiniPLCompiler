# MiniPL Compiler
 A simple Mini-PL interpreter for the course Compilers.  
 Still under development.

## Architecture

The scanner and the parser share the same pass to produce an AST. Then the semantic analyzer will process the AST. The interpreter takes the AST and also the visitor (in my project, the visitor class is the semantic analyzer) and executes the operations needed.  
The important parts in the graph are described as follow. There are also other classes and helper functions in this program, but they either belong to one of these important parts or serve as a supportive tool for the important parts.  

![img](https://lh4.googleusercontent.com/ZZt83xo6v-OnKhj9Gl7YlDiMXRV9uOSrPPgqOdPwVr7TyjBaejfRlq1MmEROk2ri780NqYeHbMTVahtSjugUCsyyymETgmbcolQmUqsTpM4U_-TC2BxV5cg70QhOA0KCsmDfZD-t)

CharacterHandler: Reading the source file, provide functions PullOne and PushOne (which works like a stack-like stream).  
Scanner:  Convert source file to a token stream. Read the source file and return a list of tokens. Ignore the comments and deal with the escape in strings. Count lines (position of the token). The scanner implemented is hard-coded. It also has a stream/stack-like function just like the CharacterHandler which can let us pull a token or push a token back to the top. When it finds an error, it just saves it to the error handler, does a little skipping and continues trying to find the next token.  
Parser: Convert tokens to an AST. Read tokens from the scanner stream provided by the previous step. The parser tries to build an AST. When it finds an error, it just saves it to the error handler, mostly skipping to the semicolon “;” and continues with the next statement. (Sometimes it has a little tolerance like trying to fix the ignoring semicolon, but anyway it always reports the error.)  
Semantic Analyzer: Do some static checks for the source codes, like type check, identifier defined or not check. It also records some useful information that may be useful for the following steps, such as type definition. In the program, it’s called “visitor”, because it’s implemented in the visitor pattern as suggested. It has many VisitXXX(xxx) functions and every AST component also has a corresponding Accept(visitor) function, in which it just simply calls the correct VisitXXX function.   
Interpreter: Actually “execute” the Pascal program. It’s also implemented in the visitor pattern. It will only be called if the Pascal program is free of errors in the previous steps. When finding a runtime error (like assert condition not met, or division by zero, …), it just simply throws an exception providing the related error information. It’s a very simple and naive interpreter, and it can be removed easily by just getting rid of using it in the main program while other parts are not affected, if necessary.   
ErrorHandler: It’s a static class that is shared by the scanner, parser, and semantic analyzer. It has some very basic functions like pushing an error, printing the errors, or clearing all the errors, …  