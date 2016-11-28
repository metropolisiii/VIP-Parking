using System.Linq;
using VIP_Parking.Models.Database;

namespace VIP_Parking.Helpers
{
    public static class DepartmentsHelper
    {
        public static int Upsert(string userDepartment)
        {
            int deptID = 0;

            //If the department isn't in the Department table, insert it
            VIPPARKINGEntities1 db = new VIPPARKINGEntities1();

            var d = db.Departments.Where(i => i.Dept_name == userDepartment).Single();
            if (d == null)
            {
                var department = new VIP_Parking.Models.Database.Department
                {
                    Dept_name = userDepartment
                };
                db.Departments.Add(department);
                db.SaveChanges();
                deptID = department.Dept_ID;
            }
            else
                deptID = d.Dept_ID;
            
            return deptID;
       }
    }
}