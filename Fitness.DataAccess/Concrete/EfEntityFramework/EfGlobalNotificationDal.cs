﻿using Fitness.Core.DataAccess.EntityFramework;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using FitnessManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DataAccess.Concrete.EfEntityFramework
{
    public class EfGlobalNotificationDal : EfEntityRepositoryBase<GlobalNotification, GymDbContext>, IGlobalNotificationDal
    {
        public EfGlobalNotificationDal(GymDbContext context) : base(context)
        {
        }
    }
}
