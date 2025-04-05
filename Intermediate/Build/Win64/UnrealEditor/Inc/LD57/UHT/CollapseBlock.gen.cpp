// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "LD57/CollapseBlock.h"
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeCollapseBlock() {}
// Cross Module References
	ENGINE_API UClass* Z_Construct_UClass_AActor();
	ENGINE_API UClass* Z_Construct_UClass_UStaticMeshComponent_NoRegister();
	LD57_API UClass* Z_Construct_UClass_ACollapseBlock();
	LD57_API UClass* Z_Construct_UClass_ACollapseBlock_NoRegister();
	UPackage* Z_Construct_UPackage__Script_LD57();
// End Cross Module References
	void ACollapseBlock::StaticRegisterNativesACollapseBlock()
	{
	}
	IMPLEMENT_CLASS_NO_AUTO_REGISTRATION(ACollapseBlock);
	UClass* Z_Construct_UClass_ACollapseBlock_NoRegister()
	{
		return ACollapseBlock::StaticClass();
	}
	struct Z_Construct_UClass_ACollapseBlock_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UECodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
#if WITH_METADATA
		static const UECodeGen_Private::FMetaDataPairParam NewProp_BlockMesh_MetaData[];
#endif
		static const UECodeGen_Private::FObjectPropertyParams NewProp_BlockMesh;
		static const UECodeGen_Private::FPropertyParamsBase* const PropPointers[];
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UECodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_ACollapseBlock_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_AActor,
		(UObject* (*)())Z_Construct_UPackage__Script_LD57,
	};
	static_assert(UE_ARRAY_COUNT(Z_Construct_UClass_ACollapseBlock_Statics::DependentSingletons) < 16);
#if WITH_METADATA
	const UECodeGen_Private::FMetaDataPairParam Z_Construct_UClass_ACollapseBlock_Statics::Class_MetaDataParams[] = {
		{ "IncludePath", "CollapseBlock.h" },
		{ "ModuleRelativePath", "CollapseBlock.h" },
	};
#endif
#if WITH_METADATA
	const UECodeGen_Private::FMetaDataPairParam Z_Construct_UClass_ACollapseBlock_Statics::NewProp_BlockMesh_MetaData[] = {
		{ "Category", "CollapseBlock" },
		{ "EditInline", "true" },
		{ "ModuleRelativePath", "CollapseBlock.h" },
	};
#endif
	const UECodeGen_Private::FObjectPropertyParams Z_Construct_UClass_ACollapseBlock_Statics::NewProp_BlockMesh = { "BlockMesh", nullptr, (EPropertyFlags)0x00100000000a0009, UECodeGen_Private::EPropertyGenFlags::Object, RF_Public|RF_Transient|RF_MarkAsNative, nullptr, nullptr, 1, STRUCT_OFFSET(ACollapseBlock, BlockMesh), Z_Construct_UClass_UStaticMeshComponent_NoRegister, METADATA_PARAMS(UE_ARRAY_COUNT(Z_Construct_UClass_ACollapseBlock_Statics::NewProp_BlockMesh_MetaData), Z_Construct_UClass_ACollapseBlock_Statics::NewProp_BlockMesh_MetaData) };
	const UECodeGen_Private::FPropertyParamsBase* const Z_Construct_UClass_ACollapseBlock_Statics::PropPointers[] = {
		(const UECodeGen_Private::FPropertyParamsBase*)&Z_Construct_UClass_ACollapseBlock_Statics::NewProp_BlockMesh,
	};
	const FCppClassTypeInfoStatic Z_Construct_UClass_ACollapseBlock_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<ACollapseBlock>::IsAbstract,
	};
	const UECodeGen_Private::FClassParams Z_Construct_UClass_ACollapseBlock_Statics::ClassParams = {
		&ACollapseBlock::StaticClass,
		"Engine",
		&StaticCppClassTypeInfo,
		DependentSingletons,
		nullptr,
		Z_Construct_UClass_ACollapseBlock_Statics::PropPointers,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		0,
		UE_ARRAY_COUNT(Z_Construct_UClass_ACollapseBlock_Statics::PropPointers),
		0,
		0x009000A4u,
		METADATA_PARAMS(UE_ARRAY_COUNT(Z_Construct_UClass_ACollapseBlock_Statics::Class_MetaDataParams), Z_Construct_UClass_ACollapseBlock_Statics::Class_MetaDataParams)
	};
	static_assert(UE_ARRAY_COUNT(Z_Construct_UClass_ACollapseBlock_Statics::PropPointers) < 2048);
	UClass* Z_Construct_UClass_ACollapseBlock()
	{
		if (!Z_Registration_Info_UClass_ACollapseBlock.OuterSingleton)
		{
			UECodeGen_Private::ConstructUClass(Z_Registration_Info_UClass_ACollapseBlock.OuterSingleton, Z_Construct_UClass_ACollapseBlock_Statics::ClassParams);
		}
		return Z_Registration_Info_UClass_ACollapseBlock.OuterSingleton;
	}
	template<> LD57_API UClass* StaticClass<ACollapseBlock>()
	{
		return ACollapseBlock::StaticClass();
	}
	DEFINE_VTABLE_PTR_HELPER_CTOR(ACollapseBlock);
	ACollapseBlock::~ACollapseBlock() {}
	struct Z_CompiledInDeferFile_FID_Unreal_Projects_LD57_Source_LD57_CollapseBlock_h_Statics
	{
		static const FClassRegisterCompiledInInfo ClassInfo[];
	};
	const FClassRegisterCompiledInInfo Z_CompiledInDeferFile_FID_Unreal_Projects_LD57_Source_LD57_CollapseBlock_h_Statics::ClassInfo[] = {
		{ Z_Construct_UClass_ACollapseBlock, ACollapseBlock::StaticClass, TEXT("ACollapseBlock"), &Z_Registration_Info_UClass_ACollapseBlock, CONSTRUCT_RELOAD_VERSION_INFO(FClassReloadVersionInfo, sizeof(ACollapseBlock), 4278421298U) },
	};
	static FRegisterCompiledInInfo Z_CompiledInDeferFile_FID_Unreal_Projects_LD57_Source_LD57_CollapseBlock_h_1083458724(TEXT("/Script/LD57"),
		Z_CompiledInDeferFile_FID_Unreal_Projects_LD57_Source_LD57_CollapseBlock_h_Statics::ClassInfo, UE_ARRAY_COUNT(Z_CompiledInDeferFile_FID_Unreal_Projects_LD57_Source_LD57_CollapseBlock_h_Statics::ClassInfo),
		nullptr, 0,
		nullptr, 0);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
