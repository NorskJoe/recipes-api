# Features (CQRS)

Application logic is organized by feature. Each feature has Commands (writes)
and Queries (reads). Handlers run Dapper directly through IDbConnectionFactory.

Example layout for the Recipes feature:

```
Features/
  Recipes/
    Commands/
      CreateRecipe/
        CreateRecipeCommand.cs         (IRequest<T> + input data)
        CreateRecipeCommandHandler.cs  (IRequestHandler<> + Dapper ExecuteAsync)
      UpdateRecipe/
      DeleteRecipe/
    Queries/
      GetRecipeBySlug/
        GetRecipeBySlugQuery.cs        (IRequest<RecipeDto>)
        GetRecipeBySlugQueryHandler.cs (Dapper QueryAsync)
        RecipeDto.cs                   (flat shape returned to the API)
      ListRecipes/
  Ingredients/
  Tags/
```

Rules:
- Handlers coordinate the use case (validate, run SQL, map to DTO).
- Queries return DTOs, not Domain entities.
- No SQL Server types here - only IDbConnectionFactory + Dapper extension methods.
