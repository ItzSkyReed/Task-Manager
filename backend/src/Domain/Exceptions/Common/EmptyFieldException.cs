using Domain.Exceptions.Base;

namespace Domain.Exceptions.Common;

public class EmptyFieldException(string field)
    : DomainBadRequestException($"Поле {field} не может быть пустым", field);