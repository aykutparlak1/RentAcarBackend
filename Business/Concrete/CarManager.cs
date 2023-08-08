﻿using Business.Abstract;
using Business.Constants;
using Core.Utilities.Business;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class CarManager : ICarService
    {

        ICarImageService _carImageService;
        ICarDal _carDal;

        public CarManager(ICarDal carDal, ICarImageService carImageService)
        {
            _carDal = carDal;
            _carImageService = carImageService;
        }
        public IResult Add(Car car)
        {
            IResult result = BusinessRules.Run(CheckPlateNumber(car.PlateNumber));
            if (result != null)
            {
                return result;
            }
            //_carImageService.SetDefaultImage(car.Id);
            _carDal.Add(car);
            return new SuccesResult(Messages.Added);

        }

        public IResult Delete(Car car)
        {
            var result = BusinessRules.Run(IfCarExists(car.Id));
            if (result != null)
            {
                return result;
            }
            _carDal.Delete(car);
            return new SuccesResult(Messages.Deleted);
        }
        public IResult Update(Car car)
        {
            var result = BusinessRules.Run(IfCarExists(car.Id));
            if (result != null)
            {
                return result;
            }
            _carDal.Update(car);
            return new SuccesResult(Messages.Updated);
        }
        public IDataResult<Car> GetByPlateNumber(string plateNumber)
        {
            var result = _carDal.Get(c=>c.PlateNumber==plateNumber);
            if (result == null)
            {
                return new ErrorDataResult<Car>(Messages.CarNotFound);
            }
            return new SuccesDataResult<Car>(result);
        }
        public IDataResult<List<Car>> GetAll()
        {
            var result = _carDal.GetAll();
            if (result.Count == 0)
            { 
                return new ErrorDataResult<List<Car>>(Messages.CarNotFound);
            }
            return new SuccesDataResult<List<Car>>(result,Messages.Listed);
        }

        public IDataResult<Car> GetById(int carId)
        {
            var result = _carDal.Get(c => c.Id == carId);
            if (result == null)
            {
                return new ErrorDataResult<Car>(Messages.CarNotFound);
            }
            return new SuccesDataResult<Car>(result, Messages.Listed);
        }

        public IDataResult<List<CarDetailDto>> GetAllCarsDetails()
        {
            var result = _carDal.GetAllCarsDetails();
            if (result.Count == 0)
            {
                return new ErrorDataResult<List<CarDetailDto>>(Messages.CarNotFound);
            }
            return new SuccesDataResult<List<CarDetailDto>>(_carDal.GetAllCarsDetails(), Messages.Listed);
        }

        public IDataResult<List<Car>> GetCarsByBrandId(int brandId)
        {
            var result = _carDal.GetAll(c => c.BrandId == brandId);
            if (result.Count == 0)
            {
                return new ErrorDataResult<List<Car>>(Messages.CarNotFound);
            }
            return new SuccesDataResult< List<Car>> (result, Messages.Listed);
        }

        public IDataResult<List<Car>> GetCarsByColorId(int colorId)
        {
            var result = _carDal.GetAll(c => c.ColorId == colorId);
            if (result.Count == 0)
            {
                return new ErrorDataResult<List<Car>>(Messages.CarNotFound);
            }
            return new SuccesDataResult<List<Car>>(result, Messages.Listed);
        }
        public IDataResult<List<CarDetailDto>> GetCarsDetailsByColorId(int id)
        {
            var result = _carDal.GetCarsDetailsByColorId(id);
            if (result.Count == 0)
            {
                return new ErrorDataResult<List<CarDetailDto>>(Messages.CarNotFound);
            }
            return new SuccesDataResult<List<CarDetailDto>>(result, Messages.Listed);
        }

        public IDataResult<List<CarDetailDto>> GetCarsDetailsByBrandAndColorId(int brandId, int colorId)
        {
            var result = _carDal.GetCarsDetailsByBrandAndColorId(brandId, colorId);
            if (result.Count == 0)
            {
                return new ErrorDataResult<List<CarDetailDto>>(Messages.CarNotFound);
            }
            return new SuccesDataResult<List<CarDetailDto>>(_carDal.GetCarsDetailsByBrandAndColorId(brandId, colorId), Messages.Listed);
        }

        public IDataResult<List<CarDetailDto>> GetCarsDetailsByBrandId(int id)
        {
            var result = _carDal.GetCarsDetailsByBrandId(id);
            if (result.Count == 0)
            {
                return new ErrorDataResult<List<CarDetailDto>>(Messages.CarNotFound);
            }
            return new SuccesDataResult<List<CarDetailDto>>(_carDal.GetCarsDetailsByBrandId(id), Messages.Listed);
        }


        private IResult CheckPlateNumber(string plateNumber)
        {
            var result = _carDal.Get(c => c.PlateNumber == plateNumber);
            if (result != null)
            {
                return new ErrorResult(Messages.PlateNumberAlreadyExists);
            }
            return new SuccesResult();
        }
        private IResult IfCarExists(int id)
        {
            var result = _carDal.Get(c => c.Id == id);
            if (result != null)
            {
                return new ErrorResult(Messages.CarNotFound);
            }
            return new SuccesResult();
        }
    }
}
