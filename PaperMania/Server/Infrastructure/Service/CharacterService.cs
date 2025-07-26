﻿using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;

    public CharacterService(ICharacterRepository characterRepository)
    {
        _characterRepository = characterRepository;
    }
    
    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int? userId)
    {
        return await _characterRepository.GetPlayerCharacterDataByUserIdAsync(userId);
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        bool exists = await _characterRepository.IsNewCharacterExistAsync(data.Id, data.CharacterId);
        if (exists)
            throw new InvalidOperationException("이미 해당 캐릭터를 보유 중입니다.");
        
        return await _characterRepository.AddPlayerCharacterDataByUserIdAsync(data);
    }
}