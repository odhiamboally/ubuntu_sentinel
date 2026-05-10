using System.Collections.Concurrent;
using US.SharedKernel.Contracts.Intake;

namespace US.Api.Features.Intake;

public sealed class UssdSessionStore
{
    private readonly ConcurrentDictionary<string, UssdSubmissionDraft> _drafts = new();

    public UssdSubmissionDraft Get(string sessionId)
    {
        return _drafts.GetOrAdd(sessionId, _ => new UssdSubmissionDraft());
    }

    public void Set(string sessionId, UssdSubmissionDraft draft)
    {
        _drafts[sessionId] = draft;
    }

    public void Remove(string sessionId)
    {
        _drafts.TryRemove(sessionId, out _);
    }
}
