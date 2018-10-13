using System.Diagnostics;
using Righthand.Immutable;

namespace KzsRest.Engine.Models
{
    [DebuggerDisplay("{Id,nq}")]
    public readonly struct DomResultItem
    {
        public string Id { get; }
        public string Content { get; }

        public DomResultItem(string id, string content)
        {
            Id = id;
            Content = content;
        }

        public DomResultItem Clone(Param<string>? id = null, Param<string>? content = null)
        {
            return new DomResultItem(id.HasValue ? id.Value.Value : Id,
				content.HasValue ? content.Value.Value : Content);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var o = (DomResultItem)obj;
            return Equals(Id, o.Id) && Equals(Content, o.Content);}

        public override int GetHashCode()
        {
            unchecked
			{
				int hash = base.GetHashCode();
				hash = hash * 37 + (Id != null ? Id.GetHashCode() : 0);
				hash = hash * 37 + (Content != null ? Content.GetHashCode() : 0);
				return hash;
			}
        }
    }
}
