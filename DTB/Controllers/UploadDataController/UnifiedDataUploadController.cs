using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTB.Data;
using DTB.Data.BatteryData;
using DTB.Data.BatteryData.BaseModel;
using System.Text.Json;
using Route = Microsoft.AspNetCore.Mvc.RouteAttribute;
using DTB.Controllers;
using DTB.Data.Devices;
using OneOf.Types;

[ApiController]
[Route("api/uploadData")]
public class UnifiedDataUploadController : ControllerBase
{
    private readonly IDbContextFactory<BatteryDbContext> _contextFactory;
    private readonly IDeviceStateService _deviceStateService;

    public UnifiedDataUploadController(
        IDbContextFactory<BatteryDbContext> contextFactory,
        IDeviceStateService deviceStateService)
    {
        _contextFactory = contextFactory;
        _deviceStateService = deviceStateService;
    }
    [HttpPost("{deviceType}")]
    public async Task<ActionResult<ApiResponse<object>>> Post(string deviceType, [FromBody] JsonElement data)
    {
        try
        {
            if (data.ValueKind == JsonValueKind.Null)
            {
                return BadRequest(ApiResponse<object>.Error(400, "Data is null"));
            }

            await using var context = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Process data upload and get the processed data
                var processResult = await ProcessDeviceDataInternal(deviceType, data, context);
                if (processResult.IsError)
                {
                    return processResult.ActionResult;
                }

                // Update device state with the processed data
                var fullData = ConvertToFullBaseModel(processResult.Data, deviceType);
                await _deviceStateService.UpdateBatteryData(deviceType, new List<FullBaseModel> { fullData });

                await transaction.CommitAsync();

                // Return success response without data
                return Ok(ApiResponse<object>.Success(null, "Data saved successfully"));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error(500, $"Internal server error: {ex.Message}"));
        }
    }
    private class ProcessResult
    {
        public bool IsError { get; set; }
        public ActionResult ActionResult { get; set; }
        public object Data { get; set; }
    }

    private async Task<ProcessResult> ProcessDeviceDataInternal(string deviceType, JsonElement data, BatteryDbContext context)
    {
        var result = await ProcessDeviceDataGeneric(deviceType, data, context);

        // 如果不是成功结果，返回错误
        if (result.Result is not OkObjectResult okResult)
        {
            return new ProcessResult
            {
                IsError = true,
                ActionResult = result.Result
            };
        }

        // 确保值是 ApiResponse<object> 类型
        if (okResult.Value is not ApiResponse<object> apiResponse || apiResponse.Data == null)
        {
            return new ProcessResult
            {
                IsError = true,
                ActionResult = BadRequest(ApiResponse<object>.Error(400, "Invalid response format"))
            };
        }

        return new ProcessResult
        {
            IsError = false,
            Data = apiResponse.Data
        };
    }
    private FullBaseModel ConvertToFullBaseModel(object data, string deviceType)
    {
        var fullModel = new FullBaseModel();
        if (data == null) return fullModel;

        // 获取源对象的所有属性
        var sourceProperties = data.GetType().GetProperties();
        // 获取目标对象的所有属性
        var targetProperties = typeof(FullBaseModel).GetProperties();

        // 遍历源对象的所有属性
        foreach (var sourceProp in sourceProperties)
        {
            // 在目标对象中查找同名属性
            var targetProp = targetProperties.FirstOrDefault(p => p.Name == sourceProp.Name);

            // 如果找到同名属性且可以写入
            if (targetProp != null && targetProp.CanWrite)
            {
                try
                {
                    // 获取源属性的值并设置到目标属性
                    var value = sourceProp.GetValue(data);
                    targetProp.SetValue(fullModel, value);
                }
                catch
                {
                    // 如果设置值失败，继续处理下一个属性
                    continue;
                }
            }
        }

        return fullModel;
    }

    private async Task<ActionResult<ApiResponse<object>>> ProcessDeviceDataGeneric(string deviceType, JsonElement data, BatteryDbContext context)
    {
        return deviceType.ToLower() switch
        {
            "jellyfeeding" => await ProcessData<JellyFeedingData>(data, context, context.JellyFeedingDatas),
            "biinserting" => await ProcessData<BiInsertingData>(data, context, context.BiInsertingDatas),
            "shellinserting" => await ProcessData<ShellInsertingData>(data, context, context.ShellInsertingDatas),
            "bottomwelding1" => await ProcessData<BottomWelding1Data>(data, context, context.BottomWelding1Datas),
            "bottomwelding2" => await ProcessData<BottomWelding2Data>(data, context, context.BottomWelding2Datas),
            "necking" => await ProcessData<NeckingData>(data, context, context.NeckingDatas),
            "tiinserting" => await ProcessData<TiInsertingData>(data, context, context.TiInsertingDatas),
            "beading" => await ProcessData<BeadingData>(data, context, context.BeadingDatas),
            "shortcircuittest" => await ProcessData<ShortCircuitTestData>(data, context, context.ShortCircuitTestDatas),
            "xray" => await ProcessData<XRAYData>(data, context, context.XRAYDatas),
            "injecting" => await ProcessData<InjectingData>(data, context, context.InjectingDatas),
            "capwelding" => await ProcessData<CapWeldingData>(data, context, context.CapWeldingDatas),
            "sealing" => await ProcessData<SealingData>(data, context, context.SealingDatas),
            "plasticfilming" => await ProcessData<PlasticFilmingData>(data, context, context.PlasticFilmingDatas),
            "filmshrinking" => await ProcessData<FilmShrinkingData>(data, context, context.FilmShrinkingDatas),
            "inkjetprinting" => await ProcessData<InkjetPrintingData>(data, context, context.InkjetPrintingDatas),
            "appearanceinspection" => await ProcessData<AppearanceInspectionData>(data, context, context.AppearanceInspectionDatas),
            "precharge" => await ProcessData<PreChargeData>(data, context, context.PreChargeDatas),
            _ => BadRequest(ApiResponse<object>.Error(400, $"Invalid device type: {deviceType}"))
        };
    }


    private async Task<ActionResult<ApiResponse<object>>> ProcessData<T>(JsonElement data, BatteryDbContext context, DbSet<T> dbSet) where T : BaseModel
    {
        try
        {
            // 反序列化数据
            var modelData = JsonSerializer.Deserialize<T>(data.GetRawText());
            if (modelData == null)
            {
                return BadRequest(ApiResponse<object>.Error(400, "Failed to deserialize data"));
            }

            // 验证必要字段
            var validationResult = await ValidateModel(modelData, context);
            if (validationResult != null)
            {
                return validationResult;
            }

            // 根据不同类型进行处理
            switch (modelData)
            {
                case JellyFeedingData:
                case BiInsertingData:
                    if (modelData is JellyBaseModel jellyData)
                    {
                        await EnsureJellyRelation(jellyData.JellyCode, context);
                    }
                    
                    break;
                // Shell和Jelly关系处理
                case ShellInsertingData:
                    if (modelData is JellyShellBaseModel jellyShellData)
                    {
                        await ProcessJellyShellRelation(jellyShellData, context);
                    }
                    break;



                case BottomWelding1Data:
                case BottomWelding2Data:
                case NeckingData:
                case TiInsertingData:
                case BeadingData:
                case ShortCircuitTestData:
                case XRAYData:
                case InjectingData:
                case CapWeldingData:
                case SealingData:
                case PlasticFilmingData:
                case FilmShrinkingData:
                    if (modelData is ShellBaseModel baseShellData)
                    {
                        await EnsureShellRelation(baseShellData.ShellCode, context);
                    }
                    break;
                // Shell和Film关系处理
                case InkjetPrintingData:
                    if (modelData is ShellFilmBaseModel shellFilmData)
                    {
                        await ProcessShellFilmRelation(shellFilmData, context);
                    }
                    break;

                case AppearanceInspectionData:
                case PreChargeData:
                    if (modelData is FilmBaseModel baseFilmData)
                    {
                        await EnsureFilmRelation(baseFilmData.FilmCode, context);
                    }
                    break;
            }

            // 设置更新时间
            modelData.updateTime = DateTime.Now;

            // 添加数据
            await dbSet.AddAsync(modelData);
            await context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(modelData, "Data saved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error(500, $"Error processing data: {ex.Message}"));
        }
    }

    // 新增的辅助方法



    private async Task<ActionResult<ApiResponse<object>>?> ValidateModel(object model, BatteryDbContext context)
    {
        // 基本验证
        switch (model)
        {
            case FilmBaseModel filmModel:
                if (string.IsNullOrEmpty(filmModel.FilmCode))
                {
                    return BadRequest(ApiResponse<object>.Error(400, "FilmCode is required"));
                }
                // 根据具体类型检查重复
                switch (filmModel)
                {
                    case AppearanceInspectionData:
                        if (await context.AppearanceInspectionDatas.AnyAsync(x => x.FilmCode == filmModel.FilmCode))
                            return Conflict(ApiResponse<object>.Error(409, $"FilmCode {filmModel.FilmCode} already exists in AppearanceInspection"));
                        break;
                    case PreChargeData:
                        if (await context.PreChargeDatas.AnyAsync(x => x.FilmCode == filmModel.FilmCode))
                            return Conflict(ApiResponse<object>.Error(409, $"FilmCode {filmModel.FilmCode} already exists in PreCharge"));
                        break;
                }
                break;

            case JellyBaseModel jellyModel:
                if (string.IsNullOrEmpty(jellyModel.JellyCode))
                {
                    return BadRequest(ApiResponse<object>.Error(400, "JellyCode is required"));
                }
                // 根据具体类型检查重复
                switch (jellyModel)
                {
                    case JellyFeedingData:
                        if (await context.JellyFeedingDatas.AnyAsync(x => x.JellyCode == jellyModel.JellyCode))
                            return Conflict(ApiResponse<object>.Error(409, $"JellyCode {jellyModel.JellyCode} already exists in JellyFeeding"));
                        break;
                    case BiInsertingData:
                        if (await context.BiInsertingDatas.AnyAsync(x => x.JellyCode == jellyModel.JellyCode))
                            return Conflict(ApiResponse<object>.Error(409, $"JellyCode {jellyModel.JellyCode} already exists in BiInserting"));
                        break;
                }
                break;

            case ShellBaseModel shellModel:
                if (string.IsNullOrEmpty(shellModel.ShellCode))
                {
                    return BadRequest(ApiResponse<object>.Error(400, "ShellCode is required"));
                }
                // 根据具体类型检查重复
                switch (shellModel)
                {
                    case BottomWelding1Data:
                        if (await context.BottomWelding1Datas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in BottomWelding1"));
                        break;
                    case BottomWelding2Data:
                        if (await context.BottomWelding2Datas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in BottomWelding2"));
                        break;
                    case NeckingData:
                        if (await context.NeckingDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in Necking"));
                        break;
                    case TiInsertingData:
                        if (await context.TiInsertingDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in TiInserting"));
                        break;
                    case BeadingData:
                        if (await context.BeadingDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in Beading"));
                        break;
                    case ShortCircuitTestData:
                        if (await context.ShortCircuitTestDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in ShortCircuitTest"));
                        break;
                    case XRAYData:
                        if (await context.XRAYDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in XRAY"));
                        break;
                    case InjectingData:
                        if (await context.InjectingDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in Injecting"));
                        break;
                    case CapWeldingData:
                        if (await context.CapWeldingDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in CapWelding"));
                        break;
                    case SealingData:
                        if (await context.SealingDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in Sealing"));
                        break;
                    case PlasticFilmingData:
                        if (await context.PlasticFilmingDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in PlasticFilming"));
                        break;
                    case FilmShrinkingData:
                        if (await context.FilmShrinkingDatas.AnyAsync(x => x.ShellCode == shellModel.ShellCode))
                            return Conflict(ApiResponse<object>.Error(409, $"ShellCode {shellModel.ShellCode} already exists in FilmShrinking"));
                        break;
                }
                break;
        }
        return null;
    }

    private async Task EnsureJellyRelation(string jellyCode, BatteryDbContext context)
    {
        var batteryRelationSet = context.Set<BatteryRelation>();
        var existing = await batteryRelationSet.FirstOrDefaultAsync(x => x.JellyCode == jellyCode);

        if (existing == null)
        {
            // 如果不存在，创建新的关系记录
            await batteryRelationSet.AddAsync(new BatteryRelation
            {
                JellyCode = jellyCode,
                ShellCode = null,
                FilmCode = null
            });
            await context.SaveChangesAsync();
        }
    }

    // 确保Shell关系存在
    private async Task EnsureShellRelation(string shellCode, BatteryDbContext context)
    {
        var batteryRelationSet = context.Set<BatteryRelation>();
        var existing = await batteryRelationSet.FirstOrDefaultAsync(x => x.ShellCode == shellCode);

        if (existing == null)
        {
            // 如果不存在，创建新的关系记录
            await batteryRelationSet.AddAsync(new BatteryRelation
            {
                JellyCode = null,
                ShellCode = shellCode,
                FilmCode = null
            });
            await context.SaveChangesAsync();
        }
    }

    // 确保Film关系存在
    private async Task EnsureFilmRelation(string filmCode, BatteryDbContext context)
    {
        var batteryRelationSet = context.Set<BatteryRelation>();
        var existing = await batteryRelationSet.FirstOrDefaultAsync(x => x.FilmCode == filmCode);

        if (existing == null)
        {
            // 如果不存在，创建新的关系记录
            await batteryRelationSet.AddAsync(new BatteryRelation
            {
                JellyCode = null,
                ShellCode = null,
                FilmCode = filmCode
            });
            await context.SaveChangesAsync();
        }
    }

    // Jelly-Shell绑定处理
    private async Task ProcessJellyShellRelation(JellyShellBaseModel data, BatteryDbContext context)
    {
        var batteryRelationSet = context.Set<BatteryRelation>();

        // 检查Jelly是否已经绑定了其他Shell
        var existingJelly = await batteryRelationSet.FirstOrDefaultAsync(x =>
            x.JellyCode == data.JellyCode && x.ShellCode != null && x.ShellCode != data.ShellCode);
        if (existingJelly != null)
        {
            throw new InvalidOperationException($"JellyCode {data.JellyCode} is already bound to ShellCode {existingJelly.ShellCode}");
        }

        // 检查Shell是否已经绑定了其他Jelly
        var existingShell = await batteryRelationSet.FirstOrDefaultAsync(x =>
            x.ShellCode == data.ShellCode && x.JellyCode != null && x.JellyCode != data.JellyCode);
        if (existingShell != null)
        {
            throw new InvalidOperationException($"ShellCode {data.ShellCode} is already bound to JellyCode {existingShell.JellyCode}");
        }

        // 查找或创建关系记录
        var relation = await batteryRelationSet.FirstOrDefaultAsync(x =>
            x.JellyCode == data.JellyCode || x.ShellCode == data.ShellCode);

        if (relation == null)
        {
            // 如果都不存在，创建新记录
            await batteryRelationSet.AddAsync(new BatteryRelation
            {
                JellyCode = data.JellyCode,
                ShellCode = data.ShellCode
            });
        }
        else
        {
            // 更新现有记录
            relation.JellyCode = data.JellyCode;
            relation.ShellCode = data.ShellCode;
            batteryRelationSet.Update(relation);
        }

        await context.SaveChangesAsync();
    }

    // Shell-Film绑定处理
    private async Task ProcessShellFilmRelation(ShellFilmBaseModel data, BatteryDbContext context)
    {
        var batteryRelationSet = context.Set<BatteryRelation>();

        // 检查Shell是否存在且已经绑定了其他Film
        var existingShell = await batteryRelationSet.FirstOrDefaultAsync(x =>
            x.ShellCode == data.ShellCode && x.FilmCode != null && x.FilmCode != data.FilmCode);
        if (existingShell != null)
        {
            throw new InvalidOperationException($"ShellCode {data.ShellCode} is already bound to FilmCode {existingShell.FilmCode}");
        }

        // 检查Film是否已经绑定了其他Shell
        var existingFilm = await batteryRelationSet.FirstOrDefaultAsync(x =>
            x.FilmCode == data.FilmCode && x.ShellCode != null && x.ShellCode != data.ShellCode);
        if (existingFilm != null)
        {
            throw new InvalidOperationException($"FilmCode {data.FilmCode} is already bound to ShellCode {existingFilm.ShellCode}");
        }

        // 查找或创建关系记录
        var relation = await batteryRelationSet.FirstOrDefaultAsync(x =>
            x.ShellCode == data.ShellCode || x.FilmCode == data.FilmCode);

        if (relation == null)
        {
            // 如果都不存在，创建新记录
            await batteryRelationSet.AddAsync(new BatteryRelation
            {
                ShellCode = data.ShellCode,
                FilmCode = data.FilmCode
            });
        }
        else
        {
            // 更新现有记录
            relation.ShellCode = data.ShellCode;
            relation.FilmCode = data.FilmCode;
            batteryRelationSet.Update(relation);
        }

        await context.SaveChangesAsync();
    }
}