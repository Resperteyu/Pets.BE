API Component Tests

What’s included
- Test host factory `TestWebAppFactory` boots the API with:
  - `PetsDbContext` on EF Core InMemory DB
  - Test auth scheme (`Authorization: Test`) via `TestAuthHandler`
- Sample test `PetsControllerTests` exercises `/api/pets` with auth

Run locally
```
cd Pets.API/test/Pets.API.ComponentTest
dotnet test
```

Notes
- If routes differ (e.g., `/api/Pets` vs `/pets`), adjust paths in tests.
- Add more tests per controller (happy + 401/403/404/validation) reusing the same factory.

