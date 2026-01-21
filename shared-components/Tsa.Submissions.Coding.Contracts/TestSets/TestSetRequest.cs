using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsa.Submissions.Coding.Contracts.TestSets;

public record TestSetRequest(
    IList<TestSetValueRequest> Inputs,
    bool IsPublic,
    string Name,
    string ProblemId);
    