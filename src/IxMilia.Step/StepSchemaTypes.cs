using System;

namespace IxMilia.Step
{
    public enum StepSchemaTypes
    {
        ExplicitDraughting = 201,
        AssociativeDraghting = 202,
        ConfigControlDesign = 203,
        StructuralAnalysisDesign = 209,
        ElectronicAssemblyInterconnect = 210,
        AutomotiveDesign = 214,
        ShipArrangement = 215,
        ShipMouldedForm = 216,
        ShipStructures = 218,
        DimensionalInspectionSchema = 219,
        FunctionalDataAndSchematics = 221,
        CastParts = 223,
        FeatureBasedProcessPlanning = 224,
        BuildingDeisgn = 225,
        PlantSpatialConfiguration = 227,
        TechnicalDataPackaging = 232,
        EngineeringProperties = 235,
        FurnitureCatalogAndInteriorDesign = 236,
        IntegratedCNC = 238,
        ProductLifeCycleSupport = 239,
        ProcessPlanning = 240,
        ManagedModelBased3DEngineering = 242
    }

    internal static class StepSchemaTypeExtensions
    {
        // SHAPE_APPEARANCE_LAYERS_GROUPS
        public const string AssociativeDraghtingText = "ASSOCIATIVE_DRAUGHTING";
        public const string AutomotiveDesignText = "AUTOMOTIVE_DESIGN";
        public const string BuildingDesignText = "BUILDING_DESIGN_SCHEMA";
        public const string CastPartsText = "CAST_PARTS_SCHEMA";
        public const string ConfigControlDesignText = "CONFIG_CONTROL_DESIGN";
        public const string ConfigurationControlled3DDesignText = "AP203_CONFIGURATION_CONTROLLED_3D_DESIGN_OF_MECHANICAL_PARTS_AND_ASSEMBLIES_MIM_LF";
        public const string DimensionalInspectionText = "DIMENSIONAL_INSPECTION_SCHEMA";
        public const string ElectronicAssemblyInterconnectText = "AP210_ELECTRONIC_ASSEMBLY_INTERCONNECT_AND_PACKAGING_DESIGN_MIM_LF";
        public const string EngineeringPropertiesText = "ENGINEERING_PROPERTIES_SCHEMA";
        public const string ExplicitDraughtingText = "EXPLICIT_DRAUGHTING";
        public const string FeatureBasedProcessPlanningText = "FEATURE_BASED_PROCESS_PLANNING";
        public const string FunctionalDataAndSchematicsText = "FUNCTIONAL_DATA_AND_SCHEMATIC_REPRESENTATION_MIM_LF";
        public const string FurnitureCatalogAndInteriorDesignText = "AP236_FURNITURE_CATALOG_AND_INTERIOR_DESIGN_MIM_LF";
        public const string IntegratedCNCText = "INTEGRATED_CNC_SCHEMA";
        public const string ManagedModelBased3DEngineeringText = "AP242_MANAGED_MODEL_BASED_3D_ENGINEERING_MIM_LF";
        public const string PlantSpatialConfigurationText = "PLANT_SPATIAL_CONFIGURATION";
        public const string ProcessPlanningText = "PROCESS_PLANNING_SCHEMA";
        public const string ProductLifeCycleSupportText = "AP239_PRODUCT_LIFE_CYCLE_SUPPORT_MIM_LF";
        public const string ShipArrangementText = "SHIP_ARRANGEMENT_SCHEMA";
        public const string ShipMouldedFormText = "SHIP_MOULDED_FORM_SCHEMA";
        public const string ShipStructuresText = "SHIP_STRUCTURES_SCHEMA";
        public const string StructuralAnalysisDesignText = "STRUCTURAL_ANALYSIS_DESIGN";
        public const string TechnicalDataPackagingText = "TECHNICAL_DATA_PACKAGING";

        public static string ToSchemaName(this StepSchemaTypes type)
        {
            switch (type)
            {
                case StepSchemaTypes.AssociativeDraghting:
                    return AssociativeDraghtingText;
                case StepSchemaTypes.AutomotiveDesign:
                    return AutomotiveDesignText;
                case StepSchemaTypes.BuildingDeisgn:
                    return BuildingDesignText;
                case StepSchemaTypes.CastParts:
                    return CastPartsText;
                case StepSchemaTypes.ConfigControlDesign:
                    return ConfigControlDesignText;
                case StepSchemaTypes.DimensionalInspectionSchema:
                    return DimensionalInspectionText;
                case StepSchemaTypes.ElectronicAssemblyInterconnect:
                    return ElectronicAssemblyInterconnectText;
                case StepSchemaTypes.EngineeringProperties:
                    return EngineeringPropertiesText;
                case StepSchemaTypes.ExplicitDraughting:
                    return ExplicitDraughtingText;
                case StepSchemaTypes.FeatureBasedProcessPlanning:
                    return FeatureBasedProcessPlanningText;
                case StepSchemaTypes.FunctionalDataAndSchematics:
                    return FunctionalDataAndSchematicsText;
                case StepSchemaTypes.FurnitureCatalogAndInteriorDesign:
                    return FurnitureCatalogAndInteriorDesignText;
                case StepSchemaTypes.IntegratedCNC:
                    return IntegratedCNCText;
                case StepSchemaTypes.ManagedModelBased3DEngineering:
                    return ManagedModelBased3DEngineeringText;
                case StepSchemaTypes.PlantSpatialConfiguration:
                    return PlantSpatialConfigurationText;
                case StepSchemaTypes.ProcessPlanning:
                    return ProcessPlanningText;
                case StepSchemaTypes.ProductLifeCycleSupport:
                    return ProductLifeCycleSupportText;
                case StepSchemaTypes.ShipArrangement:
                    return ShipArrangementText;
                case StepSchemaTypes.ShipMouldedForm:
                    return ShipMouldedFormText;
                case StepSchemaTypes.ShipStructures:
                    return ShipStructuresText;
                case StepSchemaTypes.StructuralAnalysisDesign:
                    return StructuralAnalysisDesignText;
                case StepSchemaTypes.TechnicalDataPackaging:
                    return TechnicalDataPackagingText;
                default:
                    throw new ArgumentException($"Unsupported schema type '{type}'", nameof(type));
            }
        }

        public static bool TryGetSchemaTypeFromName(string schemaName, out StepSchemaTypes schemaType)
        {
            switch (schemaName)
            {
                case AssociativeDraghtingText:
                    schemaType = StepSchemaTypes.AssociativeDraghting;
                    break;
                case AutomotiveDesignText:
                    schemaType = StepSchemaTypes.AutomotiveDesign;
                    break;
                case BuildingDesignText:
                    schemaType = StepSchemaTypes.BuildingDeisgn;
                    break;
                case CastPartsText:
                    schemaType = StepSchemaTypes.CastParts;
                    break;
                case ConfigControlDesignText:
                case ConfigurationControlled3DDesignText:
                    schemaType = StepSchemaTypes.ConfigControlDesign;
                    break;
                case DimensionalInspectionText:
                    schemaType = StepSchemaTypes.DimensionalInspectionSchema;
                    break;
                case ElectronicAssemblyInterconnectText:
                    schemaType = StepSchemaTypes.ElectronicAssemblyInterconnect;
                    break;
                case EngineeringPropertiesText:
                    schemaType = StepSchemaTypes.EngineeringProperties;
                    break;
                case ExplicitDraughtingText:
                    schemaType = StepSchemaTypes.ExplicitDraughting;
                    break;
                case FeatureBasedProcessPlanningText:
                    schemaType = StepSchemaTypes.FeatureBasedProcessPlanning;
                    break;
                case FunctionalDataAndSchematicsText:
                    schemaType = StepSchemaTypes.FunctionalDataAndSchematics;
                    break;
                case FurnitureCatalogAndInteriorDesignText:
                    schemaType = StepSchemaTypes.FurnitureCatalogAndInteriorDesign;
                    break;
                case IntegratedCNCText:
                    schemaType = StepSchemaTypes.IntegratedCNC;
                    break;
                case ManagedModelBased3DEngineeringText:
                    schemaType = StepSchemaTypes.ManagedModelBased3DEngineering;
                    break;
                case PlantSpatialConfigurationText:
                    schemaType = StepSchemaTypes.PlantSpatialConfiguration;
                    break;
                case ProcessPlanningText:
                    schemaType = StepSchemaTypes.ProcessPlanning;
                    break;
                case ProductLifeCycleSupportText:
                    schemaType = StepSchemaTypes.ProductLifeCycleSupport;
                    break;
                case ShipArrangementText:
                    schemaType = StepSchemaTypes.ShipArrangement;
                    break;
                case ShipMouldedFormText:
                    schemaType = StepSchemaTypes.ShipMouldedForm;
                    break;
                case ShipStructuresText:
                    schemaType = StepSchemaTypes.ShipStructures;
                    break;
                case StructuralAnalysisDesignText:
                    schemaType = StepSchemaTypes.StructuralAnalysisDesign;
                    break;
                case TechnicalDataPackagingText:
                    schemaType = StepSchemaTypes.TechnicalDataPackaging;
                    break;
                default:
                    schemaType = default(StepSchemaTypes);
                    return false;
            }

            return true;
        }
    }
}
