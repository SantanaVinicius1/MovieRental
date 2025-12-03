namespace MovieRental.Contracts
{
    public record CreateRentalRequest(int DaysRented, int MovieId, int CustomerId, string PaymentMethod = default!);
}
