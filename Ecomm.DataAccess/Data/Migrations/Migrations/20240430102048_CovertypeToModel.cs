using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm.DataAccess.Migrations
{
    public partial class CovertypeToModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE Create_CoverType
	           @name varchar(50)
                     AS
                   	insert coverTypes values(@name)
                                                                   ");
            migrationBuilder.Sql(@"CREATE PROCEDURE Update_CoverType
                                     @id int,
                           @name varchar(50)
                                       AS 
                   update coverTypes set name=@name where id=@id");
            migrationBuilder.Sql(@"CREATE PROCEDURE Delete_CoverTypes
                                @id int
                                   as 
                        delete from coverTypes where id=@id");
            migrationBuilder.Sql(@"CREATE PROCEDURE GEt_CoverTypes
                                             As
                               select * from coverTypes");
            migrationBuilder.Sql(@"CREATE PROCEDURE GEt_CoverType
                                        @id int
                                             as
                       select * from coverTypes where id=@id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
