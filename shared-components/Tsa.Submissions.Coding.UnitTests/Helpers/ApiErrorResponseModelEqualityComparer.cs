using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.Contracts;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class ApiErrorResponseModelEqualityComparer : IEqualityComparer<ApiErrorResponse?>
{
    public bool Equals(ApiErrorResponse? x, ApiErrorResponse? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        if (x.Data.Keys.Count != y.Data.Keys.Count) return false;

        foreach (var key in x.Data.Keys)
        {
            if (!y.Data.ContainsKey(key)) return false;
            if (x.Data[key] != y.Data[key]) return false;
        }

        var errorCodesMatch = x.ErrorCode == y.ErrorCode;
        var messagesMatch = x.Message == y.Message;

        return errorCodesMatch && messagesMatch;
    }

    public int GetHashCode(ApiErrorResponse obj)
    {
        return HashCode.Combine(obj.ErrorCode, obj.Message);
    }
}
