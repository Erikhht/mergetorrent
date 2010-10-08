MustInherit Class InvokeLambda(Of InputType, OutputType)
    Public Delegate Function GenericLambdaFunctionWithParam(ByVal input As InputType) As OutputType
    Private Delegate Function InvokeLambdaFunctionCallback(ByVal f As GenericLambdaFunctionWithParam, ByVal input As InputType, ByVal c As Control) As OutputType
    Public Shared Function InvokeLambda(ByVal f As GenericLambdaFunctionWithParam, ByVal input As InputType, ByVal c As Control) As OutputType
        If c.InvokeRequired Then
            Dim d As New InvokeLambdaFunctionCallback(AddressOf InvokeLambda)
            Return DirectCast(c.Invoke(d, New Object() {f, input, c}), OutputType)
        Else
            Return f(input)
        End If
    End Function

    Public Delegate Sub GenericLambdaSubWithParam(ByVal input As InputType)
    Public Shared Sub InvokeLambda(ByVal s As GenericLambdaSubWithParam, ByVal input As InputType, ByVal c As Control)
        InvokeLambda(Function(i As InputType)
                         s(i)
                         Return Nothing
                     End Function, input, c)
    End Sub

    Public Delegate Sub GenericLambdaSub()
    Public Shared Sub InvokeLambda(ByVal s As GenericLambdaSub, ByVal c As Control)
        InvokeLambda(Function(i As InputType)
                         s()
                         Return Nothing
                     End Function, Nothing, c)
    End Sub

    Public Delegate Function GenericLambdaFunction() As OutputType
    Public Shared Function InvokeLambda(ByVal f As GenericLambdaFunction, ByVal c As Control) As OutputType
        Return InvokeLambda(Function(i As InputType) f(), Nothing, c)
    End Function
End Class
