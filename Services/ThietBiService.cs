using System;
using System.Collections.Generic;
using QLCSVCWinApp.DataAccess;
using QLCSVCWinApp.Models;

namespace QLCSVCWinApp.Services
{
    public class ThietBiService
    {
        private readonly ThietBiDAO _dao = new ThietBiDAO();

        public List<ThietBi> GetAll() => _dao.GetAll();
        public bool Add(ThietBi tb) => _dao.Add(tb);
        public string AddAndGetId(ThietBi tb) => _dao.AddAndGetId(tb);
        public bool Update(ThietBi tb) => _dao.Update(tb);
        public bool Delete(string id) => _dao.Delete(id);

        public List<ThietBi> Search(string? keyword, string? loai, string? tinhTrang,
                                    string? maPhong, DateTime? from, DateTime? to)
            => _dao.Search(keyword, loai, tinhTrang, maPhong, from, to);
    }
}
