using System;
using System.DirectoryServices.AccountManagement;

namespace EmpowerStatusSite.Helpers
{
 public static class AuthorizationHelper
 {
 // Returns true if the given domain user is an admin for this app
 public static bool IsUserAdmin(string domainUserName)
 {
 try
 {
 using (var context = new PrincipalContext(ContextType.Domain))
 {
 var usr = UserPrincipal.FindByIdentity(context, domainUserName);
 if (usr == null) return false;
 // check group membership
 if (usr.IsMemberOf(context, IdentityType.Name, "EmpowerAdmins")) return true;
 // fallback to specific users
 var allowed = new[] { "CORP\\c1wc4","CORP\\x5px9","CORP\\y5bw3","CORP\\y9yw8","CORP\\r7vr4","CORP\\v9kb5","CORP\\y5cn2","CORP\\a7aw0","CORP\\g3cn0" };
 return System.Array.IndexOf(allowed, domainUserName) >=0;
 }
 }
 catch
 {
 return false;
 }
 }
 }
}
