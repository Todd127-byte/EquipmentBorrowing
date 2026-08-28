namespace EquipmentBorrowing.Domain;

public class Student
{
    public int Id { get; }
    public string Name { get; }
    public bool IsAllowedToBorrow { get; }
    public int MaximumActiveBorrowings { get; }

    public Student(
        int id,
        string name,
        bool isAllowedToBorrow,
        int maximumActiveBorrowings)
    {
        Id = id;
        Name = name;
        IsAllowedToBorrow = isAllowedToBorrow;
        MaximumActiveBorrowings = maximumActiveBorrowings;
    }
}