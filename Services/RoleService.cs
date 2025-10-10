using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormApp.Models;

namespace WinFormApp.Services
{
    internal class RoleService
    {
        private static RoleService instance;

        public static RoleService Instance
        {
            get
            {
                if (instance == null) instance = new RoleService();
                return RoleService.instance;
            }
            private set { RoleService.instance = value; }
        }

        private RoleService() { }

        public async Task<IEnumerable<Role>> GetListRole(string rolename = null)
        {
            using (var context = new CoffeeShopContext())
            {
                var query = context.Roles
                    .Include(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(rolename))
                {
                    query = query.Where(f => f.Name.ToLower().Contains(rolename.ToLower()));
                }

                return await query.ToListAsync();
            }
        }

        public async Task InsertRole(Role role)
        {
            using (var context = new CoffeeShopContext())
            {
                bool isExist = await context.Roles.AnyAsync(f => f.Name == role.Name);

                if (isExist)
                {
                    MessageBox.Show("Role is already exist", "Insert failed");
                    return;
                }

                var newRole = new Role
                {
                    Name = role.Name,
                    IsActive = role.IsActive,
                    RolePermissions = role.RolePermissions
                };

                context.Roles.Add(newRole);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateRole(Role role)
        {
            using (var context = new CoffeeShopContext())
            {
                Role updatedRole = await context.Roles
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.Id == role.Id);

                if(updatedRole == null)
                {
                    MessageBox.Show("Role is not exist", "Update failed");
                    return;
                }

                bool isExist = await context.Roles.AnyAsync(f => f.Name == role.Name && f.Id != role.Id);

                if (isExist)
                {
                    MessageBox.Show("Role is already exist", "Update failed");
                    return;
                }

                updatedRole.Name = role.Name;
                updatedRole.IsActive = role.IsActive;

                context.RolePermissions.RemoveRange(updatedRole.RolePermissions);

                updatedRole.RolePermissions = role.RolePermissions
                    .Select(rp => new RolePermission { RoleId = role.Id, PermissionId = rp.PermissionId })
                    .ToList();

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteRole(int roleId)
        {
            using (var context = new CoffeeShopContext())
            {
                var rolePermissions = await context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();
                
                context.RolePermissions.RemoveRange(rolePermissions);

                await context.SaveChangesAsync();   

                Role role = await context.Roles
                    .Include(r => r.Accounts)
                    .FirstOrDefaultAsync(r => r.Id == roleId);

                if (role == null)
                {
                    MessageBox.Show("Role is not exist", "Delete failed");
                    return;
                }

                if (role.Accounts.Any() || role.RolePermissions.Any())
                {
                    MessageBox.Show("There are some data related to this role", "Delete failed");
                    return;
                }

                if(role.Name.ToLower().Equals("admin") || role.Name.ToUpper().Equals("ADMIN"))
                {
                    MessageBox.Show("Cannot delete admin role", "Delete failed");
                    return;
                }

                context.Roles.Remove(role);

                await context.SaveChangesAsync();
            }
        }
    }
}
