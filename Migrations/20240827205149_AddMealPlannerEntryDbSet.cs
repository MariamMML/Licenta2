using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta2.Migrations
{
    /// <inheritdoc />
    public partial class AddMealPlannerEntryDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealPlanner_ModelUser_UserId1",
                table: "MealPlanner");

            migrationBuilder.DropForeignKey(
                name: "FK_MealPlannerEntries_MealPlanner_MealPlannerId",
                table: "MealPlannerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_MealPlannerEntries_Recipes_RecipeId",
                table: "MealPlannerEntries");

            migrationBuilder.DropIndex(
                name: "IX_MealPlanner_UserId1",
                table: "MealPlanner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MealPlannerEntries",
                table: "MealPlannerEntries");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "MealPlanner");

            migrationBuilder.RenameTable(
                name: "MealPlannerEntries",
                newName: "ModelMealPlannerEntries");

            migrationBuilder.RenameIndex(
                name: "IX_MealPlannerEntries_RecipeId",
                table: "ModelMealPlannerEntries",
                newName: "IX_ModelMealPlannerEntries_RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_MealPlannerEntries_MealPlannerId",
                table: "ModelMealPlannerEntries",
                newName: "IX_ModelMealPlannerEntries_MealPlannerId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MealPlanner",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModelMealPlannerEntries",
                table: "ModelMealPlannerEntries",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanner_UserId",
                table: "MealPlanner",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlanner_ModelUser_UserId",
                table: "MealPlanner",
                column: "UserId",
                principalTable: "ModelUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelMealPlannerEntries_MealPlanner_MealPlannerId",
                table: "ModelMealPlannerEntries",
                column: "MealPlannerId",
                principalTable: "MealPlanner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelMealPlannerEntries_Recipes_RecipeId",
                table: "ModelMealPlannerEntries",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealPlanner_ModelUser_UserId",
                table: "MealPlanner");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelMealPlannerEntries_MealPlanner_MealPlannerId",
                table: "ModelMealPlannerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelMealPlannerEntries_Recipes_RecipeId",
                table: "ModelMealPlannerEntries");

            migrationBuilder.DropIndex(
                name: "IX_MealPlanner_UserId",
                table: "MealPlanner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModelMealPlannerEntries",
                table: "ModelMealPlannerEntries");

            migrationBuilder.RenameTable(
                name: "ModelMealPlannerEntries",
                newName: "MealPlannerEntries");

            migrationBuilder.RenameIndex(
                name: "IX_ModelMealPlannerEntries_RecipeId",
                table: "MealPlannerEntries",
                newName: "IX_MealPlannerEntries_RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_ModelMealPlannerEntries_MealPlannerId",
                table: "MealPlannerEntries",
                newName: "IX_MealPlannerEntries_MealPlannerId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MealPlanner",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "MealPlanner",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealPlannerEntries",
                table: "MealPlannerEntries",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanner_UserId1",
                table: "MealPlanner",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlanner_ModelUser_UserId1",
                table: "MealPlanner",
                column: "UserId1",
                principalTable: "ModelUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlannerEntries_MealPlanner_MealPlannerId",
                table: "MealPlannerEntries",
                column: "MealPlannerId",
                principalTable: "MealPlanner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlannerEntries_Recipes_RecipeId",
                table: "MealPlannerEntries",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
