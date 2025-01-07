using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta2.Migrations
{
    /// <inheritdoc />
    public partial class Migration2Licenta2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_ModelRecipeRecipieId",
                table: "Ingredients");

            migrationBuilder.RenameColumn(
                name: "RecipieName",
                table: "Recipes",
                newName: "RecipeName");

            migrationBuilder.RenameColumn(
                name: "RecipieId",
                table: "Recipes",
                newName: "RecipeId");

            migrationBuilder.RenameColumn(
                name: "ModelRecipeRecipieId",
                table: "Ingredients",
                newName: "ModelRecipeRecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_ModelRecipeRecipieId",
                table: "Ingredients",
                newName: "IX_Ingredients_ModelRecipeRecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_ModelRecipeRecipeId",
                table: "Ingredients",
                column: "ModelRecipeRecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recipes_ModelRecipeRecipeId",
                table: "Ingredients");

            migrationBuilder.RenameColumn(
                name: "RecipeName",
                table: "Recipes",
                newName: "RecipieName");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "Recipes",
                newName: "RecipieId");

            migrationBuilder.RenameColumn(
                name: "ModelRecipeRecipeId",
                table: "Ingredients",
                newName: "ModelRecipeRecipieId");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_ModelRecipeRecipeId",
                table: "Ingredients",
                newName: "IX_Ingredients_ModelRecipeRecipieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recipes_ModelRecipeRecipieId",
                table: "Ingredients",
                column: "ModelRecipeRecipieId",
                principalTable: "Recipes",
                principalColumn: "RecipieId");
        }
    }
}
