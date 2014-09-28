using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Kendo.DynamicLinq;
using MP.Data.Infrastructure;
using MP.Data.Repository;
using MP.Model.Models;
using MP.Model.SearchModels;
using MP.Models;

namespace MP.Data.Service
{
    public interface IItemService
    {
        Item GetItem(int id);
        IEnumerable<Item> GetItems();
        DataSourceResult GetItems(ItemSearchModel parameter);
        bool AddOrUpdateItem(Item item);
        bool DeleteItem(Item item);
    }
    public class ItemService : IItemService
    {
        private readonly IItemRepository itemRepository;
        private readonly IUnitOfWork unitOfWork;

        public ItemService(IItemRepository itemRepository, IUnitOfWork unitOfWork)
        {
            this.itemRepository = itemRepository;
            this.unitOfWork = unitOfWork;
        }

        public Item GetItem(int id)
        {
            return itemRepository.GetById(id);
        }
        public IEnumerable<Item> GetItems()
        {
            var items = itemRepository.GetAll();
            return items;
        }

        public DataSourceResult GetItems(ItemSearchModel parameter)
        {
            var queryable = itemRepository.GetMany(a => (a.Trip.DepartureDate >= parameter.fromDate || parameter.fromDate == DateTime.MinValue)
                && (a.Trip.DepartureDate <= parameter.toDate || parameter.toDate == DateTime.MinValue)
                && (a.Trip.DepartureTime >= parameter.fromTime || parameter.fromTime == 0)
                && (a.Trip.DepartureTime <= parameter.toTime || parameter.toTime == 0)
                && a.Trip.TripName == parameter.TripName).OrderByDescending(a => a.Id);
            var result = parameter.noPaging ? queryable.Select(Mapper.Map<ItemModel>)
                .AsQueryable()
                .ToDataSourceResult(queryable.Count(), 0, null, parameter.filter,
                new List<Aggregator> {new Aggregator {Aggregate = "sum", Field = "Fee"}}) :
                queryable.Select(Mapper.Map<ItemModel>)
                .AsQueryable()
                .ToDataSourceResult(parameter.take, parameter.skip, null, parameter.filter,
                    new List<Aggregator> {new Aggregator {Aggregate = "sum", Field = "Fee"}});
            var group =
                queryable.GroupBy(a => a.Payed).Select(a => new GroupModel { GroupName = a.Key ? "Paid" : "Unpaid", Value = a.Sum(i => i.Fee) }).ToList();

            result.Aggregates = new 
            {
                Paid = new { sum = group.FirstOrDefault(a => a.GroupName == "Paid") != null ? group.FirstOrDefault(a => a.GroupName == "Paid").Value : 0 },
                Unpaid = new { sum = group.FirstOrDefault(a => a.GroupName == "Unpaid") != null ? group.FirstOrDefault(a => a.GroupName == "Unpaid").Value : 0 },
                All = new { sum = group.Sum(a => a.Value) }
            };
            //total = itemRepository.GetMany(a => (a.Trip.DepartureDate >= parameter.fromDate || parameter.fromDate == DateTime.MinValue)
            //    && (a.Trip.DepartureDate <= parameter.toDate || parameter.toDate == DateTime.MinValue)
            //    && (a.Trip.DepartureTime >= parameter.fromTime || parameter.fromTime == 0)
            //    && (a.Trip.DepartureTime <= parameter.toTime || parameter.toTime == 0)
            //    && a.Trip.TripName == parameter.TripName).Count();
            return result;
        }

        public bool AddOrUpdateItem(Item item)
        {
            if (item.Id != 0)
            {
                itemRepository.Update(item);
                unitOfWork.Commit();
            }
            else
            {
                itemRepository.Add(item);
                unitOfWork.Commit();
            }
            return true;
        }

        public bool DeleteItem(Item item)
        {
            itemRepository.Delete(item);
            unitOfWork.Commit();
            return true;
        }
    }
}
