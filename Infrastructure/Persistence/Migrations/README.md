# Migrations

Put your `.sql` scripts here. They are executed in filename order on startup by
`DatabaseInitializer` when the schema is missing.

Suggested naming:

- `001_CreateTables.sql`     - CREATE TABLE statements matching Domain entities
- `002_SeedMeasurements.sql` - seed rows for IngredientMeasurement (one per MeasurementType)

Set each `.sql` file's build action to copy to output, OR embed as a resource,
depending on how you implement file loading in DatabaseInitializer.
