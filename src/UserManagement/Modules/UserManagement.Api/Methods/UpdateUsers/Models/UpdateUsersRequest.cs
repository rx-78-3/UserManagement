﻿namespace UserManagement.Api.Methods.UpdateUsers.Models;

internal record UpdateUsersRequest(IEnumerable<UpdateUsersCommandItem> Users);
