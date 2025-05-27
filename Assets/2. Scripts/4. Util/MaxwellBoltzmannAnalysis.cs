using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MaxwellBoltzmannAnalysis
{

    private static double BoltzmannConstant => Constants.BoltzmannConstant;
    private static double AtomicMassUnitInKg => Constants.AtomicMassUnitInKg;

    public static float InferRMSFromSpeeds(List<float> speeds)
    {
        float rms = 0;
        foreach (float speed in speeds)
        {
            rms += speed * speed;
        }
        rms /= speeds.Count;
        return Mathf.Sqrt(rms);
    }

    /// <summary>
    /// 주어진 입자 속력 리스트와 입자 질량으로부터 온도를 추론합니다.
    /// 3차원 공간을 가정합니다.
    /// </summary>
    /// <param name="speeds">N개의 입자 속력 리스트 (단위: m/s)</param>
    /// <param name="particleAtomicMass">입자 한 개의 원자량</param>
    /// <returns>추론된 온도 (단위: K)</returns>
    public static float InferTemperatureFromSpeeds(List<float> speeds, float particleAtomicMass, int degreeOfFreedom = 3)
    {
        if (speeds == null || speeds.Count == 0)
        {
            return 0;
        }
        if (particleAtomicMass <= 0)
        {
            Debug.LogError("Particle mass must be positive.");
            throw new ArgumentOutOfRangeException(nameof(particleAtomicMass), "Particle mass must be positive.");
        }
        if (degreeOfFreedom <= 0)
        {
            Debug.LogError("Degree of freedom must be positive.");
            throw new ArgumentOutOfRangeException(nameof(degreeOfFreedom), "Degree of freedom must be positive.");
        }

        double particleMass = particleAtomicMass * AtomicMassUnitInKg;

        // 1. 속력 제곱의 평균 계산 (mean square speed)
        //    계산 중 정밀도 손실을 줄이기 위해 double 사용
        double sumOfSquares = 0.0;
        foreach (float v in speeds)
        {
            sumOfSquares += (double)v * v;
        }
        double meanSquareSpeed = sumOfSquares / speeds.Count;

        // 2. 온도 계산: T = m * <v^2> / (3 * k)
        //    3은 3차원 공간에서의 자유도 수
        double temperature = (particleMass * meanSquareSpeed) / (3 * BoltzmannConstant);

        return (float)temperature; // 최종 결과는 float으로 반환
    }

    /// <summary>
    /// 주어진 총 입자 수, 온도, 입자 질량에 대해 맥스웰-볼츠만 속력 분포에 따른
    /// "속력 v에 대한 예상 입자 수 밀도 함수" (v -> N * f(v))를 반환합니다.
    /// 이 함수는 Func<double, double> 형태로, 속력(m/s)을 입력받아
    /// 해당 속력에서의 입자 수 밀도(입자 수 / (m/s))를 반환합니다.
    /// </summary>
    /// <param name="particleCount">총 입자 수 (N)</param>
    /// <param name="temperatureKelvin">절대 온도 T (K)</param>
    /// <param name="particleAtomicMass">입자 한 개의 원자량량</param>
    /// <returns>속력 v를 입력받아 N*f(v)를 반환하는 함수. 오류 시 v -> 0.0 함수 반환.</returns>
    public static Func<double, double> MaxwellBoltzmannParticleDensityFunction(
        int particleCount,
        float temperatureKelvin,
        float particleAtomicMass)
    {
        if (particleCount <= 0)
        {
            Debug.LogError("Particle count must be positive.");
            return v_speed => 0.0; // 유효하지 않은 입력 시 항상 0을 반환하는 함수
        }
        if (temperatureKelvin <= 0)
        {
            Debug.LogError("Temperature must be positive and in Kelvin.");
            return v_speed => 0.0;
        }
        if (particleAtomicMass <= 0)
        {
            Debug.LogError("Particle mass must be positive and in kg.");
            return v_speed => 0.0;
        }

        // 입력 파라미터들을 double로 변환하여 계산 정밀도 유지
        double N = particleCount;
        double T_k = temperatureKelvin;
        double m_kg = particleAtomicMass * AtomicMassUnitInKg;

        double a = Math.Sqrt(BoltzmannConstant * T_k / m_kg);

        // 클로저(closure)를 사용하여 N, T_k, m_kg 값을 캡처하는 람다 표현식 반환
        // 이 람다 표현식이 Func<double, double>의 실제 구현이 됨
        return (double speed) =>
        {
            double v_squared = speed * speed;
            // 내부 확률 밀도 함수 f(v) 계산
            double probabilityDensity = Math.Sqrt(2.0 / Math.PI) * v_squared / (a * a * a) * Math.Exp(-v_squared / (2.0f * a * a));
            // N * f(v) 반환. 이것은 dN/dv, 즉 속력 v에서의 입자 수 밀도.
            // 특정 속력 구간 [v, v+dv]의 입자 수를 얻으려면 이 값에 dv를 곱해야 함.
            return N * probabilityDensity;
        };
    }

    public static double IntegratePaticleDensity(Func<double, double> particleDensityFunction, double start, double end, int step = 1000)
    {
        double sum = 0.0;
        double stepSize = (end - start) / step;
        for (int i = 0; i < step; i++)
        {
            double x = start + i * stepSize;
            sum += particleDensityFunction(x) * stepSize;
        }

        return sum;
    }
}