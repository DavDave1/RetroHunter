using RaSetMaker.Models;

namespace RaSetMaker.Persistence.Migrations;

public interface IMigration
{
    bool Execute(Ra2DatModel model);
}
