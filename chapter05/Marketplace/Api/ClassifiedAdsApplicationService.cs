using System;
using System.Threading.Tasks;
using Marketplace.Contracts;
using Marketplace.Domain;
using Marketplace.Framework;

namespace Marketplace.Api
{
    public class ClassifiedAdsApplicationService : IApplicationService
    {
        private readonly ICurrencyLookup _currencyLookup;
        private readonly IClassifiedAdRepository _repository;

        public ClassifiedAdsApplicationService(
            IClassifiedAdRepository repository, ICurrencyLookup currencyLookup)
        {
            _repository = repository;
            _currencyLookup = currencyLookup;
        }

        public async Task HandleAsync(object command)
        {
            switch (command)
            {
                case ClassifiedAds.V1.Create cmd:
                    if (await _repository.ExistsAsync(cmd.Id.ToString()).ConfigureAwait(false))
                        throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

                    var classifiedAd = new ClassifiedAd(
                        new ClassifiedAdId(cmd.Id),
                        new UserId(cmd.OwnerId));

                    await _repository.SaveAsync(classifiedAd).ConfigureAwait(false);
                    break;

                case ClassifiedAds.V1.SetTitle cmd:
                    await HandleUpdateAsync(cmd.Id,
                        c => c.SetTitle(ClassifiedAdTitle.FromString(cmd.Title))).ConfigureAwait(false);
                    break;

                case ClassifiedAds.V1.UpdateText cmd:
                    await HandleUpdateAsync(cmd.Id,
                        c => c.UpdateText(ClassifiedAdText.FromString(cmd.Text))).ConfigureAwait(false);
                    break;

                case ClassifiedAds.V1.UpdatePrice cmd:
                    await HandleUpdateAsync(cmd.Id,
                            c => c.UpdatePrice(Price.FromDecimal(cmd.Price, cmd.Currency, _currencyLookup)))
                        .ConfigureAwait(false);
                    break;

                case ClassifiedAds.V1.RequestToPublish cmd:
                    await HandleUpdateAsync(cmd.Id,
                        c => c.RequestToPublish()).ConfigureAwait(false);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Command type {command.GetType().FullName} is unknown");
            }
        }

        private async Task HandleUpdateAsync(Guid classifiedAdId, Action<ClassifiedAd> operation)
        {
            var classifiedAd = await _repository.LoadAsync(classifiedAdId.ToString()).ConfigureAwait(false);
            if (classifiedAd == null)
                throw new InvalidOperationException($"Entity with id {classifiedAdId} cannot be found");

            operation(classifiedAd);

            await _repository.SaveAsync(classifiedAd).ConfigureAwait(false);
        }
    }
}