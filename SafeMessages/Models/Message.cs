﻿using System;
using Newtonsoft.Json;

namespace SafeMessages.Models
{
    public class Message : IComparable, IEquatable<Message>
    {
        public Message(string from, string subject, string time, string body)
        {
            From = from;
            Subject = subject;
            Time = time;
            Body = body;
        }

        [JsonProperty("from")]
        public string From { get; }

        [JsonProperty("subject")]
        public string Subject { get; }

        [JsonProperty("time")]
        public string Time { get; }

        [JsonProperty("body")]
        public string Body { get; }

        [JsonIgnore]
        public string LocalTime => Convert.ToDateTime(Time).ToString("f");

        public int CompareTo(object obj)
        {
            var other = obj as Message;
            if (other == null)
                throw new NotSupportedException();

            var thisDt = Convert.ToDateTime(Time);
            var otherDt = Convert.ToDateTime(other.Time);
            return thisDt.CompareTo(otherDt);
        }

        public bool Equals(Message other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(From, other.From) && string.Equals(Subject, other.Subject) &&
                   string.Equals(Time, other.Time) &&
                   string.Equals(Body, other.Body);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((Message)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = From != null ? From.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Time != null ? Time.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Body != null ? Body.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
