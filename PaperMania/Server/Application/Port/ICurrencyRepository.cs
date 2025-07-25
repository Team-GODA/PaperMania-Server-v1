﻿using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICurrencyRepository
{
    Task AddPlayerGoodsDataByUserIdAsync(int? userId);
    Task<PlayerGoodsData> GetPlayerGoodsDataByUserIdAsync(int? userId);
    Task UpdatePlayerGoodsDataAsync(PlayerGoodsData data);
}