using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

//TODO: Turn into code generator
[ExcludeFromCodeCoverage]
internal class ApiErrorResponseModelEqualityComparer : IEqualityComparer<ApiErrorResponseModel?>
{
    public bool Equals(ApiErrorResponseModel? x, ApiErrorResponseModel? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        var errorCodesMatch = x.ErrorCode == y.ErrorCode;
        var messagesMatch = x.Message == y.Message;

        return errorCodesMatch && messagesMatch;
    }

    public int GetHashCode(ApiErrorResponseModel obj)
    {
        return HashCode.Combine(obj.ErrorCode, obj.Message);
    }
}
