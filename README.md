# Cart Service

## Testability:

- Unit tests mock the DAL with Moq.

- Integration tests use in-memory LiteDB (MemoryStream).

- Coverage includes main actions: add, read, remove, validation.

## Extensibility (points & cost):
- Decrease item quantity. Update RemoveItemAsync to support quantity parameter. 1–2 hours
- Event publishing. Add ICartEventsPublisher and call after operations. 2–4 hours
- Database scaling. Refactor DAL to store items in separate collection. few days
- With increasing numbers of clients, migrate to other db(like MongoDb). Async interface will be helpful. 1 day.
- Multi-currency support. Add currency converter service. 1-2 days.
