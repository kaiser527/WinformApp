using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WinFormApp.Forms;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class PermissionService
    {
        private static PermissionService instance;

        public static PermissionService Instance
        {
            get
            {
                if (instance == null) instance = new PermissionService();
                return PermissionService.instance;
            }
            private set { PermissionService.instance = value; }
        }

        private PermissionService() { }

        public async Task<IEnumerable<Permission>> GetListPermission(string permissionname = null)
        {
            using (var context = new CoffeeShopContext())
            {
                var query = context.Permissions.AsQueryable();

                if (!string.IsNullOrEmpty(permissionname))
                {
                    query = query.Where(f => f.Name.ToLower().Contains(permissionname.ToLower()));
                }

                return await query.ToListAsync();
            }
        }

        public async Task InsertPermission(Permission permission)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isExist = await context.Permissions.AnyAsync(p => p.Name == permission.Name);

                if (isExist)
                {
                    Alert.ShowAlert("Permission is already exist", Alert.AlertType.Error);
                    return;
                }

                context.Permissions.Add(permission);

                await context.SaveChangesAsync();   
            }
        }

        public async Task UpdatePermission(Permission permission)
        {
            using (var context = new CoffeeShopContext())
            {
                Permission updatedPermission = await context.Permissions.FindAsync(permission.Id);

                if(updatedPermission == null)
                {
                    Alert.ShowAlert("Permission is not exist", Alert.AlertType.Error);
                    return;
                }

                bool isExist = await context.Permissions.AnyAsync(f => f.Name == permission.Name && f.Id != permission.Id);

                if (isExist)
                {
                    Alert.ShowAlert("Permission is already exist", Alert.AlertType.Error);
                    return;
                }

                updatedPermission.Name = permission.Name;
                updatedPermission.Module = permission.Module;

                await context.SaveChangesAsync();   
            }
        }

        public async Task DeletePermission(int permissionId)
        {
            using (var context = new CoffeeShopContext())
            {
                var rolePermissions = await context.RolePermissions
                   .Where(rp => rp.PermissionId == permissionId)
                   .ToListAsync();

                context.RolePermissions.RemoveRange(rolePermissions);

                await context.SaveChangesAsync();

                Permission permission = await context.Permissions
                   .Include(r => r.RolePermissions)
                   .FirstOrDefaultAsync(r => r.Id == permissionId);

                if (permission == null)
                {
                    Alert.ShowAlert("Permission is not exist", Alert.AlertType.Error);
                    return;
                }

                context.Remove(permission);

                await context.SaveChangesAsync();
            }
        }
    }
}
