using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Permission>> GetListPermission()
        {
            using (var context = new CoffeeShopContext())
            {
                return await context.Permissions.ToListAsync();
            }
        }
    }
}
