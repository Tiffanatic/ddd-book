using System;

namespace Marketplace.Domain
{
    public class UserId
    {
        public UserId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "User id cannot be empty");

            Value = value;
        }

        private Guid Value { get; }

        public static implicit operator Guid(UserId self)
        {
            return self.Value;
        }
    }
}