﻿using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IRoomService
    {
        public Task<IEnumerable<RoomDTO>> GetRooms(int page, int limit);
        public Task<MethodResultDTO> DeleteRoom(int IdRoom);

        public Task<MethodResultDTO> PostRoom(RoomDTO roomDTO);
    }
}