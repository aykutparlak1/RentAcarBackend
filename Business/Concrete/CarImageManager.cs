﻿using Business.AbstractValidator;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Cache;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Helpers.FileHelper;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        ICarImageDal _carImageDal;
        IFileHelper _fileHelper;
        public CarImageManager(ICarImageDal carImageDal, IFileHelper fileHelper)
        {
            _carImageDal = carImageDal;
            _fileHelper = fileHelper;
        }
        [SecuredOperation("CarImage.Add")]
        [ValidationAspect(typeof(CarImageValidator))]
        [CacheRemoveAspect("ICarImageService.Get")]
        [TransactionScopeAspect]
        public IResult Add(IFormFile file, CarImage carImage)
        {
            var result = BusinessRules.Run(CheckIfCarImageLimit(carImage.CarId));
            if (result != null)
            {
                return result;
            }
            carImage.ImagePath = _fileHelper.Add(file,PathConstants.ImagesPath);
            carImage.Date = DateTime.Now;
            _carImageDal.Add(carImage);
            return new SuccesResult("Resim başarıyla yüklendi");
        }


        [SecuredOperation("CarImage.Delete")]
        [CacheRemoveAspect("ICarImageService.Get")]
        [TransactionScopeAspect]
        public IResult Delete(CarImage carImage)
        {
            _fileHelper.Delete(PathConstants.ImagesPath + carImage.ImagePath);
            _carImageDal.Delete(carImage);
            return new SuccesResult();
        }



        [SecuredOperation("CarImage.Update")]
        [ValidationAspect(typeof(CarImageValidator))]
        [CacheRemoveAspect("ICarImageService.Get")]
        [TransactionScopeAspect]

        public IResult Update(IFormFile file, CarImage carImage)
        {
            carImage.ImagePath = _fileHelper.Update(file, PathConstants.ImagesPath + carImage.ImagePath, PathConstants.ImagesPath);
            _carImageDal.Update(carImage);
            return new SuccesResult();
        }
        [CacheAspect]
        public IDataResult<List<CarImage>> GetByCarId(int carId)
        {
            var result = BusinessRules.Run(CheckCarImage(carId));
            if (result != null)
            {
                return new ErrorDataResult<List<CarImage>>();
            }
            return new SuccesDataResult<List<CarImage>>(_carImageDal.GetAll(c => c.CarId == carId));
        }
        [CacheAspect]
        public IDataResult<CarImage> GetByImageId(int imageId)
        {
            return new SuccesDataResult<CarImage>(_carImageDal.Get(c => c.Id == imageId));
        }

        [CacheAspect]
        public IDataResult<List<CarImage>> GetAll()
        {
            return new SuccesDataResult<List<CarImage>>(_carImageDal.GetAll());
        }
        private IResult CheckIfCarImageLimit(int carId)
        {
            var result = _carImageDal.GetAll(c => c.CarId == carId).Count;
            if (result >= 5)
            {
                return new ErrorResult();
            }
            return new SuccesResult();
        }
        private IResult CheckCarImage(int carId)
        {
            var result = _carImageDal.GetAll(c => c.CarId == carId).Count;
            if (result > 0)
            {
                return new SuccesResult();
            }
            return new ErrorResult();
        }
    }
}
