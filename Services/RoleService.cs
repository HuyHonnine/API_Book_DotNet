﻿using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Data;
using TestWebAPI.DTOs.Role;
using TestWebAPI.Helpers;
using TestWebAPI.Models;
using TestWebAPI.Repositories.Interfaces;
using TestWebAPI.Response;
using TestWebAPI.Services.Interfaces;
using static TestWebAPI.Response.HttpStatus;

namespace TestWebAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepositories _roleRepo;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepositories roleRepo, IMapper mapper) {
            _roleRepo = roleRepo;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<AddRoleDTO>> AddRoleAsync(AddRoleDTO roleDTO)
        {
            var serviceResponse = new ServiceResponse<AddRoleDTO>();
            try
            {
                var existingRole = await _roleRepo.GetRoleByValueAsync(roleDTO.value);
                if (existingRole != null)
                {
                    serviceResponse.statusCode = EHttpType.BadRequest;
                    serviceResponse.success = false;
                    serviceResponse.message = "Role already exists.";
                    return serviceResponse;
                }

                var role = _mapper.Map<Role>(roleDTO);
                role.code = CodeGenerator.GenerateCode(roleDTO.value);
                var adddRole = await _roleRepo.AddRoleAsync(role);
                serviceResponse.statusCode = EHttpType.Success;
                serviceResponse.message = "Role added successfully.";
                serviceResponse.success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.message = $"An unexpected error occurred: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<RoleDTO>>> GetAllRoles()
        {
            var serviceResponse = new ServiceResponse<List<RoleDTO>>();
            try
            {
                var roles = await Task.FromResult(_roleRepo.GetAllRoles().ToList());
                if (roles == null)
                {
                    serviceResponse.statusCode = EHttpType.NotFound;
                    serviceResponse.success = false;
                    serviceResponse.message = "No roles found in the database.";
                }
                else
                {
                    serviceResponse.data = roles.Select(r => _mapper.Map<RoleDTO>(r)).ToList();
                    serviceResponse.statusCode = EHttpType.Success;
                    serviceResponse.success = true;
                    serviceResponse.message = "Get all roles successfully.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.message = $"An unexpected error occurred: {ex.Message}";
                serviceResponse.statusCode = EHttpType.InternalError;
            }
            return serviceResponse;

        }

        public async Task<ServiceResponse<RoleDTO>> GetRolesById(int id)
        {
            var serviceResponse = new ServiceResponse<RoleDTO>();
            try
            {
                var role = await _roleRepo.GetRolesById(id);
                if (role == null)
                {
                    serviceResponse.statusCode = EHttpType.NotFound;
                    serviceResponse.success = false;
                    serviceResponse.message = "No role found in the database.";
                }
                else
                {
                    serviceResponse.data = _mapper.Map<RoleDTO>(role);
                    serviceResponse.statusCode = EHttpType.Success;
                    serviceResponse.success = true;
                    serviceResponse.message = "Role retrieved successfully.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.message = $"An unexpected error occurred: {ex.Message}";
                serviceResponse.statusCode = EHttpType.InternalError;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<RoleDTO>> UpdateRoleAsync(int id, AddRoleDTO roleDTO)
        {
            var serviceResponse = new ServiceResponse<RoleDTO>();
            try
            {
                var role = _mapper.Map<Role>(roleDTO);
                role.code = CodeGenerator.GenerateCode(roleDTO.value);
                Role updatedRole = await _roleRepo.UpdateRoleAsync(id, role);
                if (updatedRole == null)
                {
                    serviceResponse.statusCode = EHttpType.NotFound;
                    serviceResponse.success = false;
                    serviceResponse.message = "Role not found.";
                }
                else
                {
                    serviceResponse.statusCode = EHttpType.Success;
                    serviceResponse.success = true;
                    serviceResponse.message = "Role updated successfully.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.message = $"An unexpected error occurred: {ex.Message}";
                serviceResponse.statusCode = EHttpType.InternalError;
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<RoleDTO>> DeleteRoleAsync(int id)
        {
            var serviceResponse = new ServiceResponse<RoleDTO>();
            try
            {
                var deletedRole = await _roleRepo.DeleteRoleAsync(id);
                if(deletedRole == null)
                {
                    serviceResponse.statusCode = EHttpType.NotFound;
                    serviceResponse.success = false;
                    serviceResponse.message = "Role not found.";
                }
                else
                {
                    serviceResponse.statusCode = EHttpType.Success;
                    serviceResponse.success = true;
                    serviceResponse.message = "Role delete successfully.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.message = $"An unexpected error occurred: {ex.Message}";
                serviceResponse.statusCode = EHttpType.InternalError;
            }
            return serviceResponse;
        }
    }
}