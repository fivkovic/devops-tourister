﻿namespace User.Core.Model;

public sealed class UserProfile
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public string Residence { get; set; }
    public int NumberOfReviews { get; set; }
    public double AverageRating { get; set; }

    public Review Review(UserProfile customer, int rating, string content, DateTimeOffset timestamp)
    {
        AverageRating = (NumberOfReviews * AverageRating + rating) / (NumberOfReviews + 1);
        NumberOfReviews++;
        return new Review
        {
            OwnerId = Id,
            Customer = customer,
            Rating = rating,
            Content = content,
            Timestamp = timestamp
        };
    }

    public void RemoveRating(int rating)
    {
        AverageRating = (NumberOfReviews * AverageRating - rating) / (NumberOfReviews - 1);
        NumberOfReviews--;
    }
}