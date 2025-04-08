using RetroHunter.Models;

namespace RetroHunter.Persistence.Migrations;

public interface IMigration
{
    bool Execute(Ra2DatModel model);
}
